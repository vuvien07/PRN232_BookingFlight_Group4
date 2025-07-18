using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace BookingFlightClient.Controllers
{
    public class DiscountManageController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public DiscountManageController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private void SetUserRole()
        {
            var roleIdClaim = HttpContext.User?.FindFirst("RoleId")?.Value;
            var roleNameClaim = HttpContext.User?.FindFirst(ClaimTypes.Role)?.Value;
            
            if (int.TryParse(roleIdClaim, out int roleId))
            {
                ViewBag.UserRole = roleId;
                ViewBag.RoleName = !string.IsNullOrEmpty(roleNameClaim) ? roleNameClaim : GetRoleNameFromId(roleId);
            }
            else
            {
                ViewBag.UserRole = 4; // Manager
                ViewBag.RoleName = "Manager";
            }
        }
        
        private string GetRoleNameFromId(int roleId)
        {
            return roleId switch
            {
                1 => "Admin",
                2 => "Supporter", 
                3 => "Customer",
                4 => "Manager",
                _ => "Unknown"
            };
        }

        public IActionResult Discounts()
        {
            SetUserRole();
            return View();
        }

        public IActionResult AddDiscount()
        {
            SetUserRole();
            return View();
        }

        public IActionResult DiscountDetails(int id)
        {
            SetUserRole();
            ViewBag.DiscountId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetDiscounts([FromBody] object request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }

                var apiUrl = _configuration["ServerSettings:BaseUrl"] + "/api/Manager/discounts/list";
                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                
                var response = await client.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                return Content(responseContent, "application/json");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateDiscount([FromBody] object request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }

                var apiUrl = _configuration["ServerSettings:BaseUrl"] + "/api/Manager/discounts";
                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                
                var response = await client.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                return Content(responseContent, "application/json");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDiscount(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }

                var apiUrl = _configuration["ServerSettings:BaseUrl"] + $"/api/Manager/discounts/{id}";
                var response = await client.GetAsync(apiUrl);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                return Content(responseContent, "application/json");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDiscount(int id, [FromBody] object request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }

                var apiUrl = _configuration["ServerSettings:BaseUrl"] + $"/api/Manager/discounts/{id}";
                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                
                var response = await client.PutAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                return Content(responseContent, "application/json");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }

                var apiUrl = _configuration["ServerSettings:BaseUrl"] + $"/api/Manager/discounts/{id}";
                var response = await client.DeleteAsync(apiUrl);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                return Content(responseContent, "application/json");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDiscountStatuses()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }

                var apiUrl = _configuration["ServerSettings:BaseUrl"] + "/api/Manager/discounts/statuses";
                var response = await client.GetAsync(apiUrl);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                return Content(responseContent, "application/json");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }

                var apiUrl = _configuration["ServerSettings:BaseUrl"] + "/api/Manager/customers";
                var response = await client.GetAsync(apiUrl);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                return Content(responseContent, "application/json");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
