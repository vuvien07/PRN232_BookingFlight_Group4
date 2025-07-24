using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace BookingFlightClient.Controllers
{
    public class ChatController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ChatController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewBag.CustomerId = customerId;
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/Chat/send-message";
            var response = await _httpClient.PostAsync(apiUrl, content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Json(JsonSerializer.Deserialize<object>(result));
            }                return Json(new { success = false, message = "Failed to send message" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory(int customerId, int? supporterId, int pageSize = 50, int pageNumber = 1)
        {
            try
            {
            var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/Chat/history?customerId={customerId}&supporterId={supporterId}&pageSize={pageSize}&pageNumber={pageNumber}";
            var response = await _httpClient.GetAsync(apiUrl);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Json(JsonSerializer.Deserialize<object>(result));
            }                return Json(new { success = false, message = "Failed to get chat history" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int messageId)
        {
            try
            {
            var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/Chat/mark-read/{messageId}";
            var response = await _httpClient.PutAsync(apiUrl, null);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Json(JsonSerializer.Deserialize<object>(result));
            }                return Json(new { success = false, message = "Failed to mark message as read" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    // DTO classes
    public class SendMessageRequest
    {
        public int CustomerId { get; set; }
        public int? SupporterId { get; set; }
        public string SenderType { get; set; } = "customer";
        public string MessageContent { get; set; } = null!;
    }
}
