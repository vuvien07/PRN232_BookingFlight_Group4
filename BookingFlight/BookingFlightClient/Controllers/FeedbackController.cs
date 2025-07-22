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

		public async Task<IActionResult> Index()
		{
			try
			{
				var token = GetAuthToken();
				if (!string.IsNullOrEmpty(token))
				{
					_httpClient.DefaultRequestHeaders.Authorization = 
						new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
				}

				var response = await _httpClient.GetAsync("/api/Feedback/all");
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					var feedbacks = JsonSerializer.Deserialize<List<FeedbackDisplayViewModel>>(content, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});
					return View("~/Views/Feedback/Index.cshtml", feedbacks);
				}

				return View("~/Views/Feedback/Index.cshtml", new List<FeedbackDisplayViewModel>());
			}
			catch
			{
				return View("~/Views/Feedback/Index.cshtml", new List<FeedbackDisplayViewModel>());
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
					Content = model.Content
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

		public async Task<IActionResult> MyFeedbacks()
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

				var response = await _httpClient.GetAsync("/api/Feedback/my-feedbacks");
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					var feedbacks = JsonSerializer.Deserialize<List<FeedbackDisplayViewModel>>(content, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});
					return View("~/Views/Feedback/MyFeedbacks.cshtml", feedbacks);
				}

				return View("~/Views/Feedback/MyFeedbacks.cshtml", new List<FeedbackDisplayViewModel>());
			}
			catch
			{
				return View("~/Views/Feedback/MyFeedbacks.cshtml", new List<FeedbackDisplayViewModel>());
			}
		}
	}
}
