using BookingFlightClient.Models.DTO;
using BookingFlightClient.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using static System.Net.WebRequestMethods;
using System.Text;

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

        public async Task<IActionResult> Add()
        {
            var client = httpClientFactory.CreateClient();
            var getRolesUrl = "http://localhost:5077/api/role/get-roles";
            var rolesResponse = await client.GetAsync(getRolesUrl);
            var rolesJson = await rolesResponse.Content.ReadAsStringAsync();
            var roles = JsonSerializer.Deserialize<List<RoleDTO>>(rolesJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var manageAccountAddVM = new ManageAccountAddVM
            {
                roles = roles ?? new List<RoleDTO>()
            };
            return View(manageAccountAddVM);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ManageAccountAddVM manageAccountAddVM)
        {
            // init 
            var client = httpClientFactory.CreateClient();
            // Add Authorization token if available
            if (Request.Cookies.TryGetValue("X-Access-Token", out var token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var jsonOptions = new JsonSerializerOptions
            {
                // parse to camelCase
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                // format json
                WriteIndented = true
            };

            var jsonContent = JsonSerializer.Serialize(manageAccountAddVM, jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var apiUrl = "http://localhost:5077/api/ManageAccount/add-account";
            var response = await client.PostAsync(apiUrl, content);

            var messageNotification = "";
            var responseMessage = "";

            if (response.IsSuccessStatusCode)
            {
                TempData["AlertType"] = "success";
                TempData["MessageNotification"] = "Thêm tài khoản thành công!";
                responseMessage = response.Content.ToString();
            }
            else
            {
                TempData["AlertType"] = "danger";
                TempData["MessageNotification"] = "Thêm tài khoản thất bại!";
                responseMessage = response.Content.ToString();
            }

            ViewBag.MessageNotification = messageNotification;

            return RedirectToAction(nameof(Add));
        }
    }
}
