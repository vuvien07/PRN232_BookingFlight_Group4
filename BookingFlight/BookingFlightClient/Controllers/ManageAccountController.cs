using BookingFlightClient.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BookingFlightClient.Controllers
{
    public class ManageAccountController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ManageAccountController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index()
        {
            var client = httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5077/api/ManageAccount/get-all-account");

            // Get token từ cookie và gán vào header Authorization
            if (Request.Cookies.TryGetValue("X-Access-Token", out var token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                // Nếu lỗi xác thực, chuyển hướng về trang đăng nhập
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Index", "Login");

                return View("Error");
            }

            var jsonData = await response.Content.ReadAsStringAsync();
            var accountDTOs = JsonSerializer.Deserialize<List<ResponseAccountDTO>>(jsonData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(accountDTOs);
        }
    }
}
