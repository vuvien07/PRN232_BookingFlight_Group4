using BookingFlightClient.Models;
using BookingFlightClient.Models.DTO;
using Library;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BookingFlightClient.Controllers
{
    public class ManageAccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        
        public ManageAccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // Lấy token từ cookie/session
            var token = Request.Cookies["X-Access-Token"] ?? HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Login");
            }
            
            // Lấy username từ JWT token
            var username = JwtDecoder.GetUsernameFromToken(token);
            if (string.IsNullOrEmpty(username))
            {
                // Fallback to User.Identity.Name if JWT decode fails
                username = User.Identity?.Name;
            }
            
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Login");
            }
            
            var client = _httpClientFactory.CreateClient();
            
            // Try the user profile endpoint
            var profileRequest = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5077/api/UserProfile/me");
            profileRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var profileResponse = await client.SendAsync(profileRequest);
            
            if (profileResponse.IsSuccessStatusCode)
            {
                var profileJsonData = await profileResponse.Content.ReadAsStringAsync();
                var account = JsonSerializer.Deserialize<ResponseAccountDTO>(profileJsonData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (account != null) 
                {
                    var profileModel = new ProfileViewModel
                    {
                        Username = account.Username ?? string.Empty,
                        Fullname = account.Fullname ?? string.Empty,
                        Address = account.Address ?? string.Empty,
                        PhoneNumber = account.PhoneNumber ?? string.Empty,
                        Email = account.Email ?? string.Empty,
                        Role = account.Role ?? string.Empty,
                        Status = account.Status ?? string.Empty
                    };
                    return View(profileModel);
                }
            }
            
            // Return empty profile if API fails
            return View(new ProfileViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var token = Request.Cookies["X-Access-Token"] ?? HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Login");
            }
            
            // Lấy username từ JWT token
            var username = JwtDecoder.GetUsernameFromToken(token);
            if (string.IsNullOrEmpty(username))
            {
                username = User.Identity?.Name;
            }
            
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Login");
            }
            
            var client = _httpClientFactory.CreateClient();
            var updateRequest = new ResponseAccountDTO
            {
                Username = username ?? model.Username,
                Fullname = model.Fullname,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Role = model.Role,
                Status = model.Status
            };
            
            var content = new StringContent(JsonSerializer.Serialize(updateRequest), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsync("http://localhost:5077/api/UserProfile/me", content);
            
            if (response.IsSuccessStatusCode)
                ViewBag.Message = "Profile updated successfully!";
            else
                ViewBag.Message = "Update failed!";
            
            return View(model);
        }
    }
}
