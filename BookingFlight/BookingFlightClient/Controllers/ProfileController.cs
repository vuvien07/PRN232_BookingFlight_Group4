using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using BookingFlightClient.Models.ViewModels;

namespace BookingFlightClient.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ProfileController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private string GetAuthToken()
        {
            var authToken = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
            if (string.IsNullOrEmpty(authToken))
            {
                authToken = Request.Cookies["X-Access-Token"] ?? HttpContext.Session.GetString("AuthToken");
            }
            return authToken ?? "";
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get token using the helper method
                var token = GetAuthToken();
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Index", "Login");
                }

                // Create HttpClient instance
                var httpClient = _httpClientFactory.CreateClient();
                
                // Set authorization header
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Call API to get profile
                var apiUrl = _configuration["ServerSettings:BaseUrl"] ?? "https://localhost:7232";
                var response = await httpClient.GetAsync($"{apiUrl}/api/profile");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var apiResult = JsonSerializer.Deserialize<ApiResponse<ProfileViewModel>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResult?.Success == true && apiResult.Data != null)
                    {
                        return View("~/Views/Profile/Index.cshtml", apiResult.Data);
                    }
                }

                TempData["Error"] = "Failed to load profile information";
                return View("~/Views/Profile/Index.cshtml", new ProfileViewModel());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading profile: " + ex.Message;
                return View("~/Views/Profile/Index.cshtml", new ProfileViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Please fill in all required fields correctly";
                    return RedirectToAction("Index");
                }

                // Get token using the helper method
                var token = GetAuthToken();
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Index", "Login");
                }

                // Create HttpClient instance
                var httpClient = _httpClientFactory.CreateClient();
                
                // Set authorization header
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Prepare request data
                var updateRequest = new
                {
                    FullName = model.FullName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email
                };

                var json = JsonSerializer.Serialize(updateRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call API to update profile
                var apiUrl = _configuration["ServerSettings:BaseUrl"] ?? "https://localhost:7232";
                var response = await httpClient.PutAsync($"{apiUrl}/api/profile", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Profile updated successfully";
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = "Failed to update profile";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error updating profile: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Please fill in all password fields correctly";
                    return RedirectToAction("Index");
                }

                if (model.NewPassword != model.ConfirmPassword)
                {
                    TempData["Error"] = "New password and confirm password do not match";
                    return RedirectToAction("Index");
                }

                // Get token using the helper method
                var token = GetAuthToken();
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Index", "Login");
                }

                // Create HttpClient instance
                var httpClient = _httpClientFactory.CreateClient();
                
                // Set authorization header
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Prepare request data
                var changePasswordRequest = new
                {
                    CurrentPassword = model.CurrentPassword,
                    NewPassword = model.NewPassword,
                    ConfirmPassword = model.ConfirmPassword
                };

                var json = JsonSerializer.Serialize(changePasswordRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call API to change password
                var apiUrl = _configuration["ServerSettings:BaseUrl"] ?? "https://localhost:7232";
                var response = await httpClient.PutAsync($"{apiUrl}/api/profile/change-password", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Password changed successfully";
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = "Failed to change password. Please check your current password.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error changing password: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }

    // Helper classes for API response
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
