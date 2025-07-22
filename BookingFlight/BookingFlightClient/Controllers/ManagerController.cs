using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BookingFlightClient.Models.DTO;

namespace BookingFlightClient.Controllers
{
    [Authorize(Roles = "Manager, Admin")]
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

        // Helper method để lấy dữ liệu Dashboard
        private async Task LoadDashboardDataAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var baseUrl = _configuration.GetValue<string>("ServerSettings:BaseUrl") ?? "http://localhost:5077";
                
                // Lấy dữ liệu Seat
                var seatResponse = await httpClient.GetAsync($"{baseUrl}/api/Seat");
                if (seatResponse.IsSuccessStatusCode)
                {
                    var seatJson = await seatResponse.Content.ReadAsStringAsync();
                    var seats = JsonSerializer.Deserialize<List<SeatListDTO>>(seatJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<SeatListDTO>();
                    
                    // Thống kê Seat
                    ViewBag.TotalSeats = seats.Count;
                    ViewBag.AvailableSeats = seats.Count(s => s.StatusName == "Available");
                    ViewBag.BookedSeats = seats.Count(s => s.StatusName == "Booked");
                    ViewBag.MaintenanceSeats = seats.Count(s => s.StatusName == "Maintenance");
                }
                else
                {
                    // Default values nếu API không hoạt động
                    ViewBag.TotalSeats = 0;
                    ViewBag.AvailableSeats = 0;
                    ViewBag.BookedSeats = 0;
                    ViewBag.MaintenanceSeats = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading dashboard data: {ex.Message}");
                // Default values nếu có lỗi
                ViewBag.TotalSeats = 0;
                ViewBag.AvailableSeats = 0;
                ViewBag.BookedSeats = 0;
                ViewBag.MaintenanceSeats = 0;
            }
        }

        public async Task<IActionResult> Dashboard()
        {
            SetUserRole();
            
            // Lấy dữ liệu thống kê cho Dashboard
            await LoadDashboardDataAsync();
            
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

        public IActionResult Planes()
        {
            SetUserRole();
            
            // For Manager role, we'll load planes data via API
            if (ViewBag.UserRole == 4)
            {
                ViewBag.ShowPlaneManagement = true;
            }
            else
            {
                ViewBag.ShowPlaneManagement = false;
            }
            
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

        public IActionResult AddService()
        {
            SetUserRole();
            return View();
        }

        [HttpPost("/api/manager/services")]
        public async Task<IActionResult> CreateService([FromBody] ServiceCreateRequest request)
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

                var response = await httpClient.PostAsync($"{serverBaseUrl}/api/manager/services", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    return Json(new { success = false, message = "Failed to create service" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("/api/manager/services/statuses")]
        public async Task<IActionResult> GetServiceStatuses()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var serverBaseUrl = _configuration["ServerSettings:BaseUrl"] ?? "https://localhost:7103";
                
                var response = await httpClient.GetAsync($"{serverBaseUrl}/api/manager/services/statuses");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    return Json(new { success = false, message = "Failed to fetch statuses" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("/api/manager/items")]
        public async Task<IActionResult> GetItems()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var serverBaseUrl = _configuration["ServerSettings:BaseUrl"] ?? "https://localhost:7103";
                
                var response = await httpClient.GetAsync($"{serverBaseUrl}/api/manager/items");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    return Json(new { success = false, message = "Failed to fetch items" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("/api/manager/items")]
        public async Task<IActionResult> CreateItem([FromBody] ItemCreateRequest request)
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

                var response = await httpClient.PostAsync($"{serverBaseUrl}/api/manager/items", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Failed to create item: {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public IActionResult ServiceDetails(int? id)
        {
            SetUserRole();
            
            if (!id.HasValue)
            {
                return RedirectToAction("Services");
            }
            
            ViewBag.ServiceId = id.Value;
            return View();
        }

        public IActionResult ManageFlights()
        {
            SetUserRole();
            return View();
        }

        public IActionResult AddFlight()
        {
            SetUserRole();
            return View();
        }

        public IActionResult EditFlight(int id)
        {
            SetUserRole();
            ViewBag.FlightId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetPlanes([FromBody] PlaneListRequest request)
        {
            SetUserRole();
            
            try
            {
                Console.WriteLine($"Client GetPlanes: Search='{request.Search}', StatusId={request.StatusId}");
                
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);

                // Add authorization header if needed
                var token = Request.Cookies["AccessToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var json = JsonSerializer.Serialize(request);
                Console.WriteLine($"Client request JSON: {json}");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("api/Manager/planes/list", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Client received response: {responseContent.Substring(0, Math.Min(200, responseContent.Length))}...");

                if (response.IsSuccessStatusCode)
                {
                    return Content(responseContent, "application/json");
                }
                else
                {
                    return Json(new { success = false, message = "Failed to fetch planes data" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching planes: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while fetching planes data" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlane([FromBody] PlaneCreateRequest request)
        {
            SetUserRole();
            
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);

                // Add authorization header if needed
                var token = Request.Cookies["AccessToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("api/Manager/planes", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return Content(responseContent, "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating plane: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while creating plane" });
            }
        }

        [HttpPost]
        [Route("Manager/UpdatePlane/{id}")]
        public async Task<IActionResult> UpdatePlane(int id, [FromBody] PlaneUpdateRequest request)
        {
            SetUserRole();
            
            try
            {
                Console.WriteLine($"UpdatePlane called with id: {id}");
                Console.WriteLine($"Request data: {JsonSerializer.Serialize(request)}");
                
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);

                // Add authorization header if needed
                var token = Request.Cookies["AccessToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine($"Sending PUT request to: api/Manager/planes/{id}");
                var response = await httpClient.PutAsync($"api/Manager/planes/{id}", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                Console.WriteLine($"Server response: {responseContent}");
                return Content(responseContent, "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating plane: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while updating plane" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePlane(int id)
        {
            SetUserRole();
            
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);

                // Add authorization header if needed
                var token = Request.Cookies["AccessToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await httpClient.DeleteAsync($"api/Manager/planes/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();

                return Content(responseContent, "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting plane: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while deleting plane" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CanDeletePlane(int id)
        {
            SetUserRole();
            
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);

                // Add authorization header if needed
                var token = Request.Cookies["AccessToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await httpClient.GetAsync($"api/Manager/planes/{id}/can-delete");
                var responseContent = await response.Content.ReadAsStringAsync();

                return Content(responseContent, "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if plane can be deleted: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while checking plane deletion status" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPlaneStatuses()
        {
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);

                var response = await httpClient.GetAsync("api/Manager/planes/statuses");
                var responseContent = await response.Content.ReadAsStringAsync();

                return Content(responseContent, "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching plane statuses: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while fetching plane statuses" });
            }
        }

        [HttpGet]
        [Route("Manager/GetPlane")]
        public async Task<IActionResult> GetPlane(int id)
        {
            SetUserRole();
            
            try
            {
                Console.WriteLine($"GetPlane called with id: {id}");
                
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);

                // Add authorization header if needed
                var token = Request.Cookies["AccessToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await httpClient.GetAsync($"api/Manager/planes/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();
                
                Console.WriteLine($"GetPlane response: {responseContent}");
                return Content(responseContent, "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting plane: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while getting plane details" });
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

    public class ServiceCreateRequest
    {
        public string ServiceName { get; set; } = null!;
        public string? Detail { get; set; }
        public int ManagerId { get; set; }
        public int? StatusId { get; set; }
        public List<int>? ItemIds { get; set; } = new List<int>();
    }

    public class ItemCreateRequest
    {
        public string ItemName { get; set; } = null!;
        public string? Detail { get; set; }
        public int Price { get; set; }
        public int? StatusId { get; set; }
        public string? Image { get; set; }
    }

    public class PlaneListRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public int? StatusId { get; set; }
        public int? ManagerId { get; set; }
    }

    // DTO cho Seat Dashboard Statistics
    public class SeatListDTO
    {
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public string PlaneName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
    }
}
