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
					// Always get ALL customer feedbacks, not just current user's feedbacks
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

					// Sort feedbacks by creation date (newest first)
					allFeedbacks = allFeedbacks?.OrderByDescending(f => f.CreateAt).ToList() ?? new List<FeedbackDetailViewModel>();

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

				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					var feedbackDetails = JsonSerializer.Deserialize<List<FeedbackDetailViewModel>>(content, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});

					// Pass filter values to view
					ViewBag.SearchTerm = searchTerm;
					ViewBag.Rating = rating;
					ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
					ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

					return View("~/Views/Feedback/Index.cshtml", feedbackDetails);
				}

				return View("~/Views/Feedback/Index.cshtml", new List<FeedbackDetailViewModel>());
			}
			catch
			{
				return View("~/Views/Feedback/Index.cshtml", new List<FeedbackDetailViewModel>());
			}
		}

		[HttpGet]
		public IActionResult Create(int? ticketId, string? ticketNumber)
			{
				// Check if user is logged in before showing form
				var token = GetAuthToken();
				if (string.IsNullOrEmpty(token))
				{
					TempData["ErrorMessage"] = "Bạn cần đăng nhập để gửi feedback. Vui lòng đăng nhập trước.";
					return Redirect("/Login");
				}

				var model = new FeedbackViewModel
				{
					TicketId = ticketId,
					TicketNumber = ticketNumber
				};
				return View("~/Views/Feedback/Create.cshtml", model);
			}

			[HttpPost]
			public async Task<IActionResult> Create(FeedbackViewModel model)
			{
				if (!ModelState.IsValid)
				{
					return View("~/Views/Feedback/Create.cshtml", model);
				}

				try
				{
					var token = GetAuthToken();
					Console.WriteLine($"DEBUG: Token from GetAuthToken: {token}");

					if (string.IsNullOrEmpty(token))
					{
						TempData["ErrorMessage"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại để gửi feedback";
						return Redirect("/Login");
					}

					_httpClient.DefaultRequestHeaders.Authorization =
						new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

					var feedbackDto = new
					{
						Title = model.Title,
						Rate = model.Rate,
						Content = $"[TICKET:{model.TicketId}]{model.Content}", // Embed ticketId in content
						TicketId = model.TicketId
					};

					var json = JsonSerializer.Serialize(feedbackDto);
					var content = new StringContent(json, Encoding.UTF8, "application/json");

					var response = await _httpClient.PostAsync("/api/Feedback/create", content);

					if (response.IsSuccessStatusCode)
					{
						var responseContent = await response.Content.ReadAsStringAsync();
						var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

						if (result.TryGetProperty("message", out var messageProperty))
						{
							TempData["SuccessMessage"] = messageProperty.GetString();
						}
						else
						{
							TempData["SuccessMessage"] = "Feedback đã được gửi thành công và lưu vào cơ sở dữ liệu!";
						}

						return RedirectToAction("Index");
					}
					else
					{
						var errorContent = await response.Content.ReadAsStringAsync();
						try
						{
							var errorResult = JsonSerializer.Deserialize<JsonElement>(errorContent);
							if (errorResult.TryGetProperty("message", out var errorMessageProperty))
							{
								TempData["ErrorMessage"] = errorMessageProperty.GetString();
							}
							else
							{
								TempData["ErrorMessage"] = "Có lỗi xảy ra khi gửi feedback";
							}
						}
						catch
						{
							TempData["ErrorMessage"] = "Có lỗi xảy ra khi gửi feedback";
						}

						return View("~/Views/Feedback/Create.cshtml", model);
					}
				}
				catch (Exception ex)
				{
					TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
					return View("~/Views/Feedback/Create.cshtml", model);
				}
			}

		[HttpGet]
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
					ViewBag.TotalItems = totalItems;
					ViewBag.HasPreviousPage = page > 1;
					ViewBag.HasNextPage = page < totalPages;

					return View("~/Views/Feedback/MyFeedbacks.cshtml", paginatedFeedbacks);
				}

				return View("~/Views/Feedback/MyFeedbacks.cshtml", new List<FeedbackDetailViewModel>());
			}
			catch
			{
				return View("~/Views/Feedback/MyFeedbacks.cshtml", new List<FeedbackDetailViewModel>());
			}
		}

		[HttpGet]
		public async Task<IActionResult> ViewByTicket(int ticketId, string? ticketNumber)
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

				// Call the new endpoint that returns feedback status and details
				var response = await _httpClient.GetAsync($"/api/Feedback/ticket/{ticketId}/feedback");
				
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					var feedbackStatus = JsonSerializer.Deserialize<FeedbackStatusResponse>(content, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});

					if (feedbackStatus?.HasFeedback == true && feedbackStatus.FeedbackDetails != null)
					{
						// Set ticket info for display
						feedbackStatus.FeedbackDetails.TicketId = ticketId;
						feedbackStatus.FeedbackDetails.TicketNumber = ticketNumber ?? "";
						
						return View("~/Views/Feedback/ViewDetails.cshtml", feedbackStatus.FeedbackDetails);
					}
					else
					{
						TempData["ErrorMessage"] = "Không tìm thấy feedback cho vé này.";
						return RedirectToAction("Create", new { ticketId, ticketNumber });
					}
				}
				else
				{
					TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải feedback.";
					return RedirectToAction("Create", new { ticketId, ticketNumber });
				}
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
				return RedirectToAction("Create", new { ticketId, ticketNumber });
			}
		}
	}

	// Helper class for API response
	public class FeedbackStatusResponse
	{
		public bool HasFeedback { get; set; }
		public FeedbackDetailViewModel? FeedbackDetails { get; set; }
	}
}