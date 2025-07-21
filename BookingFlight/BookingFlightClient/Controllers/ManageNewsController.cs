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

        [HttpPost]
        public async Task<IActionResult> Create(RequestAddNewsDTO newsDTO, IFormFile? imageUpload)
        {
            try
            {
                // Handle image upload if provided
                if (imageUpload != null && imageUpload.Length > 0)
                {
                    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "news");
                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }

                    var fileName = $"{Guid.NewGuid()}_{imageUpload.FileName}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageUpload.CopyToAsync(stream);
                    }

                    newsDTO.Image = $"/images/news/{fileName}";
                }

                // Get current user's account ID from session or claims
                if (Request.Cookies.TryGetValue("UserId", out var userIdStr))
                {
                    if (int.TryParse(userIdStr, out var userId))
                    {
                        newsDTO.AccountId = userId;
                    }
                }

                var client = httpClientFactory.CreateClient();

                // Add Authorization token if available
                if (Request.Cookies.TryGetValue("X-Access-Token", out var token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                var jsonContent = JsonSerializer.Serialize(newsDTO, jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("http://localhost:5077/api/managenews/news", content);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertType"] = "success";
                    TempData["MessageNotification"] = "Tạo bản tin thành công.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["AlertType"] = "danger";
                    TempData["MessageNotification"] = "Tạo bản tin thất bại.";
                    return View(newsDTO);
                }
            }
            catch (Exception ex)
            {
                TempData["AlertType"] = "danger";
                TempData["MessageNotification"] = $"Có lỗi xảy ra: {ex.Message}";
                return View(newsDTO);
            }
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
            try
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

                // Handle image upload if provided
                if (imageUpload != null && imageUpload.Length > 0)
                {
                    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "news");
                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }

                    var fileName = $"{Guid.NewGuid()}_{imageUpload.FileName}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageUpload.CopyToAsync(stream);
                    }

                    requestEditNewsDTO.Image = $"/images/news/{fileName}";
                }

                var client = httpClientFactory.CreateClient();

                // Add Authorization token if available
                if (Request.Cookies.TryGetValue("X-Access-Token", out var token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                var jsonContent = JsonSerializer.Serialize(requestEditNewsDTO, jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"http://localhost:5077/api/managenews/news/{requestEditNewsDTO.NewId}", content);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertType"] = "success";
                    TempData["MessageNotification"] = "Cập nhật bản tin thành công.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["AlertType"] = "danger";
                    TempData["MessageNotification"] = "Cập nhật bản tin thất bại.";
                    return View(newsDTO);
                }
            }
            catch (Exception ex)
            {
                TempData["AlertType"] = "danger";
                TempData["MessageNotification"] = $"Có lỗi xảy ra: {ex.Message}";
                return View(newsDTO);
            }
        }

        public async Task<IActionResult> DeleteConfirmation(int newId)
        {
            if (newId == 0)
            {
                return NotFound();
            }

            // Get news details for confirmation
            var client = httpClientFactory.CreateClient();

            // Add Authorization token if available
            if (Request.Cookies.TryGetValue("X-Access-Token", out var token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync($"http://localhost:5077/api/managenews/news/{newId}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Index", "Login");
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["AlertType"] = "danger";
                TempData["MessageNotification"] = "Không tìm thấy bản tin.";
                return RedirectToAction(nameof(Index));
            }

            var jsonData = await response.Content.ReadAsStringAsync();
            var newsDTO = JsonSerializer.Deserialize<ResponseNewsDTO>(jsonData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(newsDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int newId)
        {
            try
            {
                var client = httpClientFactory.CreateClient();

                // Add Authorization token if available
                if (Request.Cookies.TryGetValue("X-Access-Token", out var token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await client.DeleteAsync($"http://localhost:5077/api/managenews/news/{newId}");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertType"] = "success";
                    TempData["MessageNotification"] = "Xóa bản tin thành công!";
                }
                else
                {
                    TempData["AlertType"] = "danger";
                    TempData["MessageNotification"] = "Xóa bản tin thất bại. Vui lòng thử lại.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["AlertType"] = "danger";
                TempData["MessageNotification"] = $"Có lỗi xảy ra: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Remove the old DeleteConfirmed method since we're not using it anymore
        /*
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int newId)
        {
            // This method is no longer needed
        }
        */
    }
}
