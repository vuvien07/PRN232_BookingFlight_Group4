using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace BookingFlightClient.Controllers
{
    public class SupporterChatController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public SupporterChatController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var supporterId = HttpContext.Session.GetInt32("SupporterId");
            if (supporterId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewBag.SupporterId = supporterId;
            ViewBag.UserRole = 2; // Supporter role
            ViewBag.RoleName = "Supporter";
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetConversations(int supporterId)
        {
            try
            {
            var apiUrl = $"{_configuration["ApiSettings:BaseUrl"]}/Chat/supporter-conversations/{supporterId}";
            var response = await _httpClient.GetAsync(apiUrl);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Json(JsonSerializer.Deserialize<object>(result));
            }                return Json(new { success = false, message = "Failed to get conversations" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendSupporterMessageRequest request)
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
        public async Task<IActionResult> GetChatHistory(int customerId, int supporterId, int pageSize = 50, int pageNumber = 1)
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
    }

    public class SendSupporterMessageRequest
    {
        public int CustomerId { get; set; }
        public int SupporterId { get; set; }
        public string SenderType { get; set; } = "supporter";
        public string MessageContent { get; set; } = null!;
    }
}
