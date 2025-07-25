using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace BookingFlightClient.Controllers
{
    [Authorize]
    public class SeatManagementController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public SeatManagementController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private void SetUserRole()
        {
            try
            {
                var roleClaim = User.FindFirst("role")?.Value ?? User.FindFirst("Role")?.Value;
                if (string.IsNullOrEmpty(roleClaim))
                {
                    roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                }

                Console.WriteLine($"SeatManagementController: User role from claims: {roleClaim}");

                if (!string.IsNullOrEmpty(roleClaim))
                {
                    if (int.TryParse(roleClaim, out int roleId))
                    {
                        ViewBag.UserRole = GetRoleNameFromId(roleId);
                    }
                    else
                    {
                        ViewBag.UserRole = roleClaim;
                    }
                }
                else
                {
                    ViewBag.UserRole = "Unknown";
                }

                Console.WriteLine($"SeatManagementController: ViewBag.UserRole set to: {ViewBag.UserRole}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SeatManagementController: Error setting user role: {ex.Message}");
                ViewBag.UserRole = "Error";
            }
        }

        private string GetRoleNameFromId(int roleId)
        {
            return roleId switch
            {
                1 => "Admin",
                2 => "Manager",
                3 => "Customer",
                _ => "Unknown"
            };
        }

        private string GetAuthToken()
        {
            Console.WriteLine("=== DEBUG TOKEN SEARCH IN SEAT CONTROLLER ===");
            
            // Log all headers
            Console.WriteLine("All request headers:");
            foreach (var header in Request.Headers)
            {
                Console.WriteLine($"  {header.Key}: {header.Value}");
            }
            
            // Log all cookies
            Console.WriteLine("All request cookies:");
            foreach (var cookie in Request.Cookies)
            {
                Console.WriteLine($"  {cookie.Key}: {cookie.Value}");
            }
            
            // First try Authorization header (sent by JavaScript)
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader))
            {
                var authToken = authHeader.Replace("Bearer ", "");
                Console.WriteLine($"GetAuthToken: Found token in Authorization header: {authToken.Substring(0, Math.Min(20, authToken.Length))}...");
                return authToken;
            }
            
            // Then try cookie
            var cookieToken = Request.Cookies["X-Access-Token"];
            if (!string.IsNullOrEmpty(cookieToken))
            {
                Console.WriteLine($"GetAuthToken: Found token in X-Access-Token cookie: {cookieToken.Substring(0, Math.Min(20, cookieToken.Length))}...");
                return cookieToken;
            }
            
            // Finally try session
            var sessionToken = HttpContext.Session.GetString("AuthToken");
            if (!string.IsNullOrEmpty(sessionToken))
            {
                Console.WriteLine($"GetAuthToken: Found token in session: {sessionToken.Substring(0, Math.Min(20, sessionToken.Length))}...");
                return sessionToken;
            }
            
            Console.WriteLine("GetAuthToken: No token found in headers, cookies, or session");
            return "";
        }

        public IActionResult Index()
        {
            SetUserRole();
            return View();
        }

        public IActionResult FlightSeats()
        {
            SetUserRole();
            return View();
        }

        [HttpGet("api/flights/list")]
        public async Task<IActionResult> GetFlightsList()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var serverBaseUrl = _configuration["ServerSettings:BaseUrl"] ?? "http://localhost:5077";
                
                var authToken = GetAuthToken();
                Console.WriteLine($"SeatManagementController: Token found: {!string.IsNullOrEmpty(authToken)}");
                Console.WriteLine($"SeatManagementController: Token length: {authToken.Length}");
                Console.WriteLine($"SeatManagementController: Server URL: {serverBaseUrl}");
                
                // Forward all cookies from client request to server request
                foreach (var cookie in Request.Cookies)
                {
                    httpClient.DefaultRequestHeaders.Add("Cookie", $"{cookie.Key}={cookie.Value}");
                }
                
                if (!string.IsNullOrEmpty(authToken))
                {
                    httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
                }

                // Create request for flights - using same structure as FlightManage
                var request = new
                {
                    page = 1,
                    pageSize = 100
                };

                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{serverBaseUrl}/api/FlightManage/list", content);
                
                Console.WriteLine($"SeatManagementController: Response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"SeatManagementController: Response content: {responseContent}");
                    return Content(responseContent, "application/json");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"SeatManagementController: Error response: {errorContent}");
                    return StatusCode((int)response.StatusCode, new { success = false, message = $"Server error: {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SeatManagementController: Exception: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("api/flights/{flightId}/seats")]
        public async Task<IActionResult> GetFlightSeats(int flightId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var serverBaseUrl = _configuration["ServerSettings:BaseUrl"] ?? "http://localhost:5077";
                
                var authToken = GetAuthToken();
                if (!string.IsNullOrEmpty(authToken))
                {
                    httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
                }

                // Forward all cookies from client request to server request
                foreach (var cookie in Request.Cookies)
                {
                    httpClient.DefaultRequestHeaders.Add("Cookie", $"{cookie.Key}={cookie.Value}");
                }

                var response = await httpClient.GetAsync($"{serverBaseUrl}/api/Seat/flight/{flightId}/seats");
                
                Console.WriteLine($"SeatManagementController: Seats response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"SeatManagementController: Seats response content: {responseContent}");
                    return Content(responseContent, "application/json");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"SeatManagementController: Seats error response: {errorContent}");
                    return StatusCode((int)response.StatusCode, new { success = false, message = $"Server error: {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SeatManagementController: Seats exception: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
