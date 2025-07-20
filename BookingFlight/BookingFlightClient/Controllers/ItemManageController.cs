using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace BookingFlightClient.Controllers
{
    public class ItemManageController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ItemManageController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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
                2 => "Customer", 
                3 => "Supporter",
                4 => "Manager",
                _ => "Unknown"
            };
        }

        public IActionResult Items()
        {
            SetUserRole();
            return View();
        }

        public IActionResult AddItem()
        {
            SetUserRole();
            return View();
        }

        public IActionResult ItemDetails(int id)
        {
            SetUserRole();
            ViewBag.ItemId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetItems([FromBody] object request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }

                var apiUrl = _configuration["ServerSettings:BaseUrl"] + "/api/Manager/items/list";
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
        public async Task<IActionResult> CreateItem([FromBody] object request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }

                var apiUrl = _configuration["ServerSettings:BaseUrl"] + "/api/Manager/items";
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
        public async Task<IActionResult> GetItem(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }

                var apiUrl = _configuration["ServerSettings:BaseUrl"] + $"/api/Manager/items/{id}";
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
        public async Task<IActionResult> UpdateItem(int id, [FromBody] object request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }

                var apiUrl = _configuration["ServerSettings:BaseUrl"] + $"/api/Manager/items/{id}";
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
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }

                var apiUrl = _configuration["ServerSettings:BaseUrl"] + $"/api/Manager/items/{id}";
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
        public async Task<IActionResult> GetItemStatuses()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }

                var apiUrl = _configuration["ServerSettings:BaseUrl"] + "/api/Manager/items/statuses";
                var response = await client.GetAsync(apiUrl);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                return Content(responseContent, "application/json");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile imageFile)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    return Json(new { success = false, message = "Please select a file to upload." });
                }

                // Validate file type
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(imageFile.ContentType.ToLower()))
                {
                    return Json(new { success = false, message = "Only image files (JPEG, PNG, GIF) are allowed." });
                }

                // Validate file size (max 5MB)
                if (imageFile.Length > 5 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "File size cannot exceed 5MB." });
                }

                // Create upload directory if it doesn't exist
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                // Generate unique filename
                var fileExtension = Path.GetExtension(imageFile.FileName);
                var fileName = $"item_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString("N")[..8]}{fileExtension}";
                var filePath = Path.Combine(uploadsDir, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Return the relative URL for the uploaded image
                var imageUrl = $"/images/{fileName}";
                return Json(new { success = true, imageUrl = imageUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Upload failed: {ex.Message}" });
            }
        }
    }
}
