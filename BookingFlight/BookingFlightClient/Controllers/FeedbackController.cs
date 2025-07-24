using Microsoft.AspNetCore.Mvc;
using BookingFlightClient.Models.ViewModels;
using System.Text.Json;
using System.Text;
using System.Security.Claims;

namespace BookingFlightClient.Controllers
{
	public class FeedbackController : Controller
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration _configuration;

		public FeedbackController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClient = httpClientFactory.CreateClient();
			_configuration = configuration;
			_httpClient.BaseAddress = new Uri(_configuration["ServerSettings:BaseUrl"] ?? "http://localhost:5077");
		}

		private string? GetAuthToken()
		{
			// First try to get from X-Access-Token cookie (primary method)
			var cookieToken = Request.Cookies["X-Access-Token"];
			if (!string.IsNullOrEmpty(cookieToken))
			{
				return cookieToken;
			}

			// Fallback to session JwtToken (legacy method)
			var sessionToken = HttpContext.Session.GetString("JwtToken");
			return sessionToken;
		}

		public async Task<IActionResult> Index(string? searchText = null, int? rating = null, int page = 1, int pageSize = 10)
		{
			try
			{
				var token = GetAuthToken();
				if (!string.IsNullOrEmpty(token))
				{
					_httpClient.DefaultRequestHeaders.Authorization = 
						new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
				}

				string apiUrl;
				if (!string.IsNullOrEmpty(searchText) || rating.HasValue)
				{
					// Build query string for search
					var queryParams = new List<string>();
					if (!string.IsNullOrEmpty(searchText))
						queryParams.Add($"searchTerm={Uri.EscapeDataString(searchText)}");
					if (rating.HasValue)
						queryParams.Add($"rating={rating.Value}");

					var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
					apiUrl = $"/api/Feedback/search{queryString}";
				}
				else
				{
					apiUrl = "/api/Feedback/detail/all";
				}

				var response = await _httpClient.GetAsync(apiUrl);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					var allFeedbacks = JsonSerializer.Deserialize<List<FeedbackDetailViewModel>>(content, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});

					// Apply pagination
					var totalItems = allFeedbacks?.Count ?? 0;
					var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
					var paginatedFeedbacks = allFeedbacks?
						.Skip((page - 1) * pageSize)
						.Take(pageSize)
						.ToList() ?? new List<FeedbackDetailViewModel>();

					// Pass pagination info to view
					ViewBag.CurrentPage = page;
					ViewBag.TotalPages = totalPages;
					ViewBag.PageSize = pageSize;
					ViewBag.TotalItems = totalItems;
					ViewBag.HasPreviousPage = page > 1;
					ViewBag.HasNextPage = page < totalPages;

					// Pass filter values to view
					ViewBag.SearchText = searchText;
					ViewBag.Rating = rating;

					// Pass all feedbacks to view for statistics (not just paginated ones)
					ViewData["AllFeedbacks"] = allFeedbacks;

					return View("~/Views/Feedback/Index.cshtml", paginatedFeedbacks);
				}

				// Pass filter values and pagination info even if API call fails
				ViewBag.SearchText = searchText;
				ViewBag.Rating = rating;
				ViewBag.CurrentPage = 1;
				ViewBag.TotalPages = 0;
				ViewBag.PageSize = pageSize;
				ViewBag.TotalItems = 0;
				ViewBag.HasPreviousPage = false;
				ViewBag.HasNextPage = false;
				ViewData["AllFeedbacks"] = new List<FeedbackDetailViewModel>();
				return View("~/Views/Feedback/Index.cshtml", new List<FeedbackDetailViewModel>());
			}
			catch
			{
				// Pass filter values and pagination info even if exception occurs
				ViewBag.SearchText = searchText;
				ViewBag.Rating = rating;
				ViewBag.CurrentPage = 1;
				ViewBag.TotalPages = 0;
				ViewBag.PageSize = pageSize;
				ViewBag.TotalItems = 0;
				ViewBag.HasPreviousPage = false;
				ViewBag.HasNextPage = false;
				ViewData["AllFeedbacks"] = new List<FeedbackDetailViewModel>();
				return View("~/Views/Feedback/Index.cshtml", new List<FeedbackDetailViewModel>());
			}
		}

		[HttpGet]
		public async Task<IActionResult> Search(string? searchTerm = null, int? rating = null, DateTime? fromDate = null, DateTime? toDate = null)
		{
			try
			{
				var token = GetAuthToken();
				if (!string.IsNullOrEmpty(token))
				{
					_httpClient.DefaultRequestHeaders.Authorization =
						new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
				}

				// Build query string
				var queryParams = new List<string>();
				if (!string.IsNullOrEmpty(searchTerm))
					queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
				if (rating.HasValue)
					queryParams.Add($"rating={rating.Value}");
				if (fromDate.HasValue)
					queryParams.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");
				if (toDate.HasValue)
					queryParams.Add($"toDate={toDate.Value:yyyy-MM-dd}");

				var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
				var response = await _httpClient.GetAsync($"/api/Feedback/search{queryString}");

				List<FeedbackDetailViewModel> feedbackDetails = new List<FeedbackDetailViewModel>();
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					feedbackDetails = JsonSerializer.Deserialize<List<FeedbackDetailViewModel>>(content, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});
				}

				// Pass filter values to view
				ViewBag.SearchTerm = searchTerm;
				ViewBag.Rating = rating;
				ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
				ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

				// Pass all feedbacks to view for statistics (not just paginated ones)
				ViewData["AllFeedbacks"] = feedbackDetails ?? new List<FeedbackDetailViewModel>();

				return View("~/Views/Feedback/Index.cshtml", feedbackDetails);
			}
			catch
			{
				ViewData["AllFeedbacks"] = new List<FeedbackDetailViewModel>();
				return View("~/Views/Feedback/Index.cshtml", new List<FeedbackDetailViewModel>());
			}
		}

		public async Task<IActionResult> MyFeedbacks(int page = 1, int pageSize = 10)
		{
			try
			{
				var token = GetAuthToken();
				if (string.IsNullOrEmpty(token))
				{
					return Redirect("/Login");
				}

				_httpClient.DefaultRequestHeaders.Authorization = 
					new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				// Get only user's own feedback details
				var response = await _httpClient.GetAsync("/api/Feedback/detail/my-feedbacks");
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					var myFeedbacks = JsonSerializer.Deserialize<List<FeedbackDetailViewModel>>(content, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});

					// Apply pagination
					var totalItems = myFeedbacks?.Count ?? 0;
					var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
					var paginatedFeedbacks = myFeedbacks?
						.Skip((page - 1) * pageSize)
						.Take(pageSize)
						.ToList() ?? new List<FeedbackDetailViewModel>();

					// Pass pagination info to view
					ViewBag.CurrentPage = page;
					ViewBag.TotalPages = totalPages;
					ViewBag.PageSize = pageSize;
					ViewBag.TotalCount = totalItems;
					ViewBag.HasPreviousPage = page > 1;
					ViewBag.HasNextPage = page < totalPages;

					return View("~/Views/Feedback/MyFeedbacks.cshtml", paginatedFeedbacks);
				}

				// Pass pagination info even if API call fails
				ViewBag.CurrentPage = 1;
				ViewBag.TotalPages = 0;
				ViewBag.PageSize = pageSize;
				ViewBag.TotalCount = 0;
				ViewBag.HasPreviousPage = false;
				ViewBag.HasNextPage = false;
				return View("~/Views/Feedback/MyFeedbacks.cshtml", new List<FeedbackDetailViewModel>());
			}
			catch
			{
				// Pass pagination info even if exception occurs
				ViewBag.CurrentPage = 1;
				ViewBag.TotalPages = 0;
				ViewBag.PageSize = pageSize;
				ViewBag.TotalCount = 0;
				ViewBag.HasPreviousPage = false;
				ViewBag.HasNextPage = false;
				return View("~/Views/Feedback/MyFeedbacks.cshtml", new List<FeedbackDetailViewModel>());
			}
		}

		// Lấy feedback detail theo ticket ID
		public async Task<IActionResult> GetFeedbackDetailByTicket(int ticketId)
		{
			try
			{
				var token = GetAuthToken();
				if (string.IsNullOrEmpty(token))
				{
					TempData["ErrorMessage"] = "Bạn cần đăng nhập để xem feedback.";
					return Redirect("/Login");
				}

				_httpClient.DefaultRequestHeaders.Authorization = 
					new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				var response = await _httpClient.GetAsync($"/api/Feedback/detail/by-ticket/{ticketId}");
				
				if (response.IsSuccessStatusCode)
				{
					var jsonContent = await response.Content.ReadAsStringAsync();
					var feedbackDetails = JsonSerializer.Deserialize<List<FeedbackDetailViewModel>>(jsonContent, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});

					ViewBag.TicketId = ticketId;
					return View("~/Views/Feedback/FeedbackDetail.cshtml", feedbackDetails);
				}
				else
				{
					TempData["ErrorMessage"] = "Không thể tải thông tin feedback cho vé này.";
					return RedirectToAction("Index", "BookingHistory");
				}
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
				return RedirectToAction("Index", "BookingHistory");
			}
		}

		// Lấy tất cả feedback detail của user hiện tại
		public async Task<IActionResult> MyFeedbackDetails()
		{
			try
			{
				var token = GetAuthToken();
				if (string.IsNullOrEmpty(token))
				{
					TempData["ErrorMessage"] = "Bạn cần đăng nhập để xem feedback.";
					return Redirect("/Login");
				}

				_httpClient.DefaultRequestHeaders.Authorization = 
					new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				var response = await _httpClient.GetAsync("/api/Feedback/detail/my-feedbacks");
				
				if (response.IsSuccessStatusCode)
				{
					var jsonContent = await response.Content.ReadAsStringAsync();
					var feedbackDetails = JsonSerializer.Deserialize<List<FeedbackDetailViewModel>>(jsonContent, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});

					return View("~/Views/Feedback/MyFeedbackDetails.cshtml", feedbackDetails);
				}
				else
				{
					TempData["ErrorMessage"] = "Không thể tải danh sách feedback.";
				}
				
				return View("~/Views/Feedback/MyFeedbackDetails.cshtml", new List<FeedbackDetailViewModel>());
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
				return View("~/Views/Feedback/MyFeedbackDetails.cshtml", new List<FeedbackDetailViewModel>());
			}
		}

		// API để lấy feedback theo ticket ID (dành cho Ajax)
		[HttpGet]
		public async Task<IActionResult> GetFeedbackByTicket(int ticketId)
		{
			try
			{
				var token = GetAuthToken();
				if (string.IsNullOrEmpty(token))
				{
					return Json(new { success = false, message = "Bạn cần đăng nhập." });
				}

				_httpClient.DefaultRequestHeaders.Authorization = 
					new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				var response = await _httpClient.GetAsync($"/api/Feedback/by-ticket/{ticketId}");
				
				if (response.IsSuccessStatusCode)
				{
					var jsonContent = await response.Content.ReadAsStringAsync();
					var feedbacks = JsonSerializer.Deserialize<List<FeedbackDisplayViewModel>>(jsonContent, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});

					return Json(new { success = true, data = feedbacks });
				}
				else
				{
					return Json(new { success = false, message = "Không tìm thấy feedback cho vé này." });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
			}
		}

		// API để lấy feedback detail theo ticket ID (dành cho Ajax)
		[HttpGet]
		public async Task<IActionResult> GetFeedbackDetailByTicketApi(int ticketId)
		{
			try
			{
				var token = GetAuthToken();
				if (string.IsNullOrEmpty(token))
				{
					return Json(new { success = false, message = "Bạn cần đăng nhập." });
				}

				_httpClient.DefaultRequestHeaders.Authorization = 
					new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				var response = await _httpClient.GetAsync($"/api/Feedback/detail/by-ticket/{ticketId}");
				
				if (response.IsSuccessStatusCode)
				{
					var jsonContent = await response.Content.ReadAsStringAsync();
					var feedbackDetails = JsonSerializer.Deserialize<List<FeedbackDetailViewModel>>(jsonContent, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});

					return Json(new { success = true, data = feedbackDetails });
				}
				else
				{
					return Json(new { success = false, message = "Không tìm thấy feedback detail cho vé này." });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
			}
		}

		[HttpGet]
		public async Task<IActionResult> ViewByTicket(int ticketId, string? ticketNumber)
		{
			try
			{
				Console.WriteLine($"[DEBUG] ViewByTicket called with ticketId: {ticketId}, ticketNumber: {ticketNumber}");
				
				var token = GetAuthToken();
				if (string.IsNullOrEmpty(token))
				{
					Console.WriteLine("[DEBUG] No token found, redirecting to login");
					return RedirectToAction("Index", "Login");
				}

				// Get account ID from token
				var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
				{
					// Try to get from JWT token directly
					accountId = Library.JwtDecoder.GetAccountIdFromToken(token) ?? 0;
					if (accountId == 0)
					{
						Console.WriteLine("[DEBUG] Cannot determine account ID");
						TempData["ErrorMessage"] = "Không thể xác định tài khoản.";
						return RedirectToAction("Index", "Login");
					}
				}

				Console.WriteLine($"[DEBUG] Account ID: {accountId}");

				_httpClient.DefaultRequestHeaders.Authorization = 
					new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				var apiUrl = $"/api/Feedback/detail/by-ticket/{ticketId}?accountId={accountId}";
				Console.WriteLine($"[DEBUG] API URL: {apiUrl}");
				
				var response = await _httpClient.GetAsync(apiUrl);
				Console.WriteLine($"[DEBUG] API Response Status: {response.StatusCode}");
				
				if (response.IsSuccessStatusCode)
				{
					var jsonContent = await response.Content.ReadAsStringAsync();
					Console.WriteLine($"[DEBUG] API Response Content: {jsonContent}");
					
					var feedbackDetails = JsonSerializer.Deserialize<List<FeedbackDetailViewModel>>(jsonContent, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});

					Console.WriteLine($"[DEBUG] Deserialized feedback count: {feedbackDetails?.Count ?? 0}");

					if (feedbackDetails != null && feedbackDetails.Any())
					{
						var feedback = feedbackDetails.First();
						ViewBag.TicketNumber = ticketNumber;
						Console.WriteLine($"[DEBUG] Returning ViewDetails with feedback ID: {feedback.FeedbackId}");
						return View("~/Views/Feedback/ViewDetails.cshtml", feedback);
					}
					else
					{
						Console.WriteLine("[DEBUG] No feedback found, redirecting to Create");
						// No feedback found, redirect to Create feedback page
						return RedirectToAction("Create", "Feedback", new { ticketId, ticketNumber });
					}
				}
				else
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					Console.WriteLine($"[DEBUG] API Error Content: {errorContent}");
				}
				
				Console.WriteLine("[DEBUG] No feedback found or API error, redirecting to Create");
				// No feedback found or API error, redirect to Create feedback page
				TempData["InfoMessage"] = "Chưa có feedback cho vé này. Vui lòng tạo feedback mới.";
				return RedirectToAction("Create", "Feedback", new { ticketId, ticketNumber });
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[DEBUG] Exception in ViewByTicket: {ex.Message}");
				Console.WriteLine($"[DEBUG] Stack trace: {ex.StackTrace}");
				TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
				return RedirectToAction("Create", "Feedback", new { ticketId, ticketNumber });
			}
		}

		// GET: Feedback/Create
		public async Task<IActionResult> Create(int? ticketId, string? ticketNumber)
		{
			try
			{
				var token = GetAuthToken();
				if (string.IsNullOrEmpty(token))
				{
					TempData["ErrorMessage"] = "Vui lòng đăng nhập để tạo feedback.";
					return RedirectToAction("Login", "Login");
				}

				// Set authorization header
				_httpClient.DefaultRequestHeaders.Authorization = 
					new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				var viewModel = new FeedbackDetailViewModel();

				// If ticketId is provided, get ticket information
				if (ticketId.HasValue)
				{
					var apiUrl = $"/api/Ticket/{ticketId.Value}";
					var response = await _httpClient.GetAsync(apiUrl);

					if (response.IsSuccessStatusCode)
					{
						var ticketJson = await response.Content.ReadAsStringAsync();
						var ticketData = JsonSerializer.Deserialize<dynamic>(ticketJson);
						
						viewModel.TicketId = ticketId.Value;
						viewModel.TicketNumber = ticketNumber ?? "";
					}
					else
					{
						viewModel.TicketId = ticketId.Value;
						viewModel.TicketNumber = ticketNumber ?? "";
					}
				}

				return View(viewModel);
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
				return RedirectToAction("Index", "Home");
			}
		}

		// POST: Feedback/Create
		[HttpPost]
		public async Task<IActionResult> Create(FeedbackDetailViewModel model)
		{
			try
			{
				var token = GetAuthToken();
				if (string.IsNullOrEmpty(token))
				{
					TempData["ErrorMessage"] = "Vui lòng đăng nhập để tạo feedback.";
					return RedirectToAction("Login", "Login");
				}

				if (!ModelState.IsValid)
				{
					return View(model);
				}

				// Set authorization header
				_httpClient.DefaultRequestHeaders.Authorization = 
					new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				// Create feedback DTO
				var createFeedbackDto = new
				{
					title = model.Title,
					content = model.Content,
					rate = model.Rate
				};

				var json = JsonSerializer.Serialize(createFeedbackDto);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				// Get account ID from claims
				var accountIdClaim = HttpContext.User.FindFirst("AccountId")?.Value;
				if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
				{
					TempData["ErrorMessage"] = "Không thể xác định thông tin tài khoản.";
					return View(model);
				}

				// Call API to create feedback
				var response = await _httpClient.PostAsync($"/api/Feedback/create/{accountId}", content);

				if (response.IsSuccessStatusCode)
				{
					var responseContent = await response.Content.ReadAsStringAsync();
					var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
					
					if (result.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
					{
						// If we have ticketId, create FeedbackTicket relationship
						if (model.TicketId > 0)
						{
							if (result.TryGetProperty("data", out var dataProp) && 
								dataProp.TryGetProperty("feedbackId", out var feedbackIdProp))
							{
								var feedbackId = feedbackIdProp.GetInt32();
								
								// Create FeedbackTicket relationship
								var feedbackTicketDto = new
								{
									feedbackId = feedbackId,
									ticketId = model.TicketId,
									createdAt = DateTime.Now
								};

								var feedbackTicketJson = JsonSerializer.Serialize(feedbackTicketDto);
								var feedbackTicketContent = new StringContent(feedbackTicketJson, Encoding.UTF8, "application/json");
								
								var feedbackTicketResponse = await _httpClient.PostAsync("/api/FeedbackTicket/create", feedbackTicketContent);
								
								if (!feedbackTicketResponse.IsSuccessStatusCode)
								{
									Console.WriteLine($"Warning: Failed to create FeedbackTicket relationship");
								}
							}
						}

						TempData["SuccessMessage"] = "Tạo feedback thành công!";
						return RedirectToAction("Index");
					}
					else
					{
						TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạo feedback.";
						return View(model);
					}
				}
				else
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					TempData["ErrorMessage"] = "Lỗi khi tạo feedback: " + errorContent;
					return View(model);
				}
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
				return View(model);
			}
		}
	}
}
