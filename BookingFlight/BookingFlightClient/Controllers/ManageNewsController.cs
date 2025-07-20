using BookingFlightClient.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BookingFlightClient.Controllers
{
    public class ManageNewsController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ManageNewsController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index()
        {
            // call the API to get the list of news
            var client = httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5077/api/managenews/news");

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
            var newsDTOs = JsonSerializer.Deserialize<List<ResponseNewsDTO>>(jsonData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            // and pass it to the view
            return View(newsDTOs);
        }
    }
}
