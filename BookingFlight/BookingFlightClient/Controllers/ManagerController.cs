using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace BookingFlightClient.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ManagerController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private void SetUserRole()
        {
            // Get role from JWT token claims
            var roleIdClaim = HttpContext.User?.FindFirst("RoleId")?.Value;
            var roleNameClaim = HttpContext.User?.FindFirst(ClaimTypes.Role)?.Value;
            
            // Debug logging
            Console.WriteLine($"ManagerController SetUserRole - RoleId claim: {roleIdClaim}");
            Console.WriteLine($"ManagerController SetUserRole - Role name claim: {roleNameClaim}");
            
            if (int.TryParse(roleIdClaim, out int roleId))
            {
                ViewBag.UserRole = roleId;
                
                // Map roleId to correct roleName if roleName claim is missing or incorrect
                string correctRoleName = GetRoleNameFromId(roleId);
                ViewBag.RoleName = !string.IsNullOrEmpty(roleNameClaim) ? roleNameClaim : correctRoleName;
                
                Console.WriteLine($"ManagerController SetUserRole - Set ViewBag.UserRole = {roleId}, ViewBag.RoleName = {ViewBag.RoleName}");
                Console.WriteLine($"ManagerController SetUserRole - Original role name claim: {roleNameClaim}, Mapped role name: {correctRoleName}");
            }
            else
            {
                // Default to role 4 (Manager) if not found - corrected based on actual database
                ViewBag.UserRole = 4;
                ViewBag.RoleName = "Manager";
                Console.WriteLine($"ManagerController SetUserRole - Using default: ViewBag.UserRole = 4, ViewBag.RoleName = Manager");
            }
        }
        
        private string GetRoleNameFromId(int roleId)
        {
            // Database mapping: 1=Admin, 2=Supporter, 3=Customer, 4=Manager
            return roleId switch
            {
                1 => "Admin",
                2 => "Supporter", 
                3 => "Customer",
                4 => "Manager",
                _ => "Unknown"
            };
        }

        public IActionResult Dashboard()
        {
            SetUserRole();
            return View();
        }

        public IActionResult Services()
        {
            SetUserRole();
            
            // For Manager role, we'll load services data via API
            if (ViewBag.UserRole == 4)
            {
                ViewBag.ShowServiceManagement = true;
            }
            else
            {
                ViewBag.ShowServiceManagement = false;
            }
            
            return View();
        }

        public IActionResult ManageItems()
        {
            SetUserRole();
            return View();
        }

        public IActionResult Reports()
        {
            SetUserRole();
            return View();
        }

        public IActionResult Profile()
        {
            SetUserRole();
            return View();
        }

        [HttpPost("/api/manager/services/list")]
        public async Task<IActionResult> GetServicesList([FromBody] ServiceListRequest request)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var serverBaseUrl = _configuration["ServerSettings:BaseUrl"] ?? "https://localhost:7103";
                
                // Get JWT token from request headers
                var authToken = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
                if (string.IsNullOrEmpty(authToken))
                {
                    // Try to get from cookie or session
                    authToken = Request.Cookies["X-Access-Token"] ?? HttpContext.Session.GetString("AuthToken");
                }

                if (!string.IsNullOrEmpty(authToken))
                {
                    httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
                }

                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{serverBaseUrl}/api/manager/services/list", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    return Json(new { success = false, message = "Failed to fetch services from server" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    // DTO classes for API requests
    public class ServiceListRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public int? StatusId { get; set; }
        public int? ManagerId { get; set; }
    }
}
