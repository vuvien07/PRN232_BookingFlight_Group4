using BookingFlightClient.Models.DTO;
using BookingFlightClient.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Text;
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

        public async Task<IActionResult> Create()
        {

            return View();
        }

        public async Task<IActionResult> Details(int newId)
        {
            Console.WriteLine($"Client :: ManageNewsController :: Details :: newId = {newId}");
            // call the API to get the news by id
            if (newId == null || newId == 0)
            {
                return NotFound();
            }
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
            var apiUrl = $"http://localhost:5077/api/managenews/news/{newId}";
            var response = await client.GetAsync(apiUrl);
            var responseNewsDTO = new ResponseNewsDTO();
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Index", "Login");
            }
            if (!response.IsSuccessStatusCode)
            {
                TempData["AlertType"] = "danger";
                TempData["MessageNotification"] = "Lấy bản tin thất bại.";
                return View(responseNewsDTO);
            }
            TempData["AlertType"] = "success";
            TempData["MessageNotification"] = "Lấy bản tin thành công.";
            var jsonData = await response.Content.ReadAsStringAsync();
            responseNewsDTO = JsonSerializer.Deserialize<ResponseNewsDTO>(jsonData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return View(responseNewsDTO);
        }

        public async Task<IActionResult> Edit(int newId)
        {
            Console.WriteLine($"Client :: ManageNewsController :: Edit :: newId = {newId}");

            // call the API to get the news by id
            if (newId == null || newId == 0)
            {
                return NotFound();
            }

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

            var apiUrl = $"http://localhost:5077/api/managenews/news/{newId}";
            var response = await client.GetAsync(apiUrl);
            var responseNewsDTO = new ResponseNewsDTO();

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Index", "Login");
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["AlertType"] = "danger";
                TempData["MessageNotification"] = "Lấy bản tin thất bại.";
                return View(responseNewsDTO);
            }
            TempData["AlertType"] = "success";
            TempData["MessageNotification"] = "Lấy bản tin thành công.";
            var jsonData = await response.Content.ReadAsStringAsync();
            responseNewsDTO = JsonSerializer.Deserialize<ResponseNewsDTO>(jsonData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(responseNewsDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ResponseNewsDTO newsDTO, IFormFile? imageUpload)
        {
            RequestUpdateNewsDTO requestEditNewsDTO = new RequestUpdateNewsDTO
            {
                NewId = newsDTO.NewId,
                Title = newsDTO.Title,
                Content = newsDTO.Content,
                Category = newsDTO.Category,
                Author = newsDTO.Author,
                Image = newsDTO.Image                                                             
            };



            return RedirectToAction(nameof(Index));
        }
    }
}
