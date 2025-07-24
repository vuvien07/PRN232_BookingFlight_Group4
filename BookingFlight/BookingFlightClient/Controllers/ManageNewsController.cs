using BookingFlightClient.Models.DTO;
using BookingFlightClient.Models.ViewModels;
using BookingFlightClient.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BookingFlightClient.Controllers
{
    public class ManageNewsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IS3Service _s3Service;

        public ManageNewsController(IHttpClientFactory httpClientFactory, IS3Service s3Service)
        {
            _httpClientFactory = httpClientFactory;
            _s3Service = s3Service;
        }

        private bool IsUserAuthenticated()
        {
            return Request.Cookies.TryGetValue("X-Access-Token", out var token) && !string.IsNullOrEmpty(token);
        }

        private string? GetAccessToken()
        {
            Request.Cookies.TryGetValue("X-Access-Token", out var token);
            return token;
        }
        public async Task<IActionResult> Index()
        {
            // call the API to get the list of news
            var client = _httpClientFactory.CreateClient();

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
            // Check if user is authenticated
            if (!IsUserAuthenticated())
            {
                TempData["AlertType"] = "warning";
                TempData["MessageNotification"] = "Bạn cần đăng nhập để truy cập chức năng này.";
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RequestAddNewsDTO newsDTO, IFormFile? imageUpload)
        {
            //Console.WriteLine($"Client :: ManageNews :: Create :: {newsDTO.ToString()} :: {imageUpload.FileName}");
            try
            {

                // Handle image upload if provided
                if (imageUpload != null && imageUpload.Length > 0)
                {
                    // Check if the uploaded file is an image
                    var validImageTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                    if (!validImageTypes.Contains(imageUpload.ContentType))
                    {
                        TempData["AlertType"] = "danger";
                        TempData["MessageNotification"] = "Chỉ hỗ trợ định dạng ảnh JPG, JPEG, PNG, hoặc GIF.";
                        return View(newsDTO);
                    }

                    // Create file name with a unique identifier
                    var fileName = $"images/news/{imageUpload.FileName}";

                    // Upload the file to S3
                    using var stream = imageUpload.OpenReadStream();
                    var fileUrl = await _s3Service.UploadFileAsync(fileName, stream);

                    // Assign the file URL to the newsDTO
                    newsDTO.Image = fileUrl;
                }

                var client = _httpClientFactory.CreateClient();

                // Check if token exists in cookies
                var token = GetAccessToken();
                if (string.IsNullOrEmpty(token))
                {
                    TempData["AlertType"] = "warning";
                    TempData["MessageNotification"] = "Bạn chưa đăng nhập. Vui lòng đăng nhập để tiếp tục.";
                    return RedirectToAction("Index", "Login");
                }

                // Add Authorization token
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);



                newsDTO.AccountId = 1; // Assuming the account ID is 1 for the current user, you can modify this as needed

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
            var client = _httpClientFactory.CreateClient();
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
            var client = _httpClientFactory.CreateClient();
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
                    // Validate image type
                    var validImageTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                    if (!validImageTypes.Contains(imageUpload.ContentType))
                    {
                        TempData["AlertType"] = "danger";
                        TempData["MessageNotification"] = "Chỉ hỗ trợ định dạng ảnh JPG, JPEG, PNG, hoặc GIF.";
                        return View(newsDTO);
                    }

                    // Delete old image from S3 if it exists
                    if (!string.IsNullOrEmpty(requestEditNewsDTO.Image))
                    {
                        // Extract the S3 key from the URL (e.g., images/news/1701fc8e-695f-40ae-ad32-94d086643ede_Titanic.jpg)
                        var oldImageKey = requestEditNewsDTO.Image.Replace(
                            "https://thanhnd-s3-bucket-store-prn232.s3.ap-southeast-1.amazonaws.com/", "");
                        await _s3Service.DeleteFileAsync(oldImageKey);
                    }

                    // Upload new image to S3
                    var fileName = $"images/news/{imageUpload.FileName}";
                    using var stream = imageUpload.OpenReadStream();
                    var fileUrl = await _s3Service.UploadFileAsync(fileName, stream);
                    requestEditNewsDTO.Image = fileUrl;
                }

                var client = _httpClientFactory.CreateClient();
                var token = GetAccessToken();
                if (string.IsNullOrEmpty(token))
                {
                    TempData["AlertType"] = "warning";
                    TempData["MessageNotification"] = "Bạn chưa đăng nhập. Vui lòng đăng nhập để tiếp tục.";
                    return RedirectToAction("Index", "Login");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
            var client = _httpClientFactory.CreateClient();

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
                var client = _httpClientFactory.CreateClient();

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
