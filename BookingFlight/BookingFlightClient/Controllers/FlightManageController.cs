using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace BookingFlightClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightManageController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public FlightManageController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private string GetAuthToken()
        {
            Console.WriteLine("=== DEBUG TOKEN SEARCH IN CONTROLLER ===");
            
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

        [HttpPost("list")]
        public async Task<IActionResult> GetFlightsList([FromBody] FlightListRequest request)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var serverBaseUrl = _configuration["ServerSettings:BaseUrl"] ?? "http://localhost:5077";
                
                var authToken = GetAuthToken();
                Console.WriteLine($"FlightManageController: Token found: {!string.IsNullOrEmpty(authToken)}");
                Console.WriteLine($"FlightManageController: Token length: {authToken.Length}");
                Console.WriteLine($"FlightManageController: Server URL: {serverBaseUrl}");
                
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

                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{serverBaseUrl}/api/FlightManage/list", content);
                
                Console.WriteLine($"FlightManageController: Response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"FlightManageController: Error response: {errorContent}");
                    return StatusCode((int)response.StatusCode, new { success = false, message = $"Server error: {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FlightManageController: Exception: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetFlightStatuses()
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

                var response = await httpClient.GetAsync($"{serverBaseUrl}/api/FlightManage/statuses");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, new { success = false, message = "Failed to fetch flight statuses" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("check-conflicts")]
        public async Task<IActionResult> CheckFlightConflicts([FromBody] FlightConflictRequest request)
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

                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{serverBaseUrl}/api/FlightManage/check-conflicts", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { success = false, message = $"Server error: {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFlight(int id)
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

                var response = await httpClient.GetAsync($"{serverBaseUrl}/api/FlightManage/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, new { success = false, message = "Failed to fetch flight details" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlight([FromBody] FlightCreateRequest request)
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

                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{serverBaseUrl}/api/FlightManage", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { success = false, message = $"Server error: {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFlight(int id, [FromBody] FlightUpdateRequest request)
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

                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"{serverBaseUrl}/api/FlightManage/{id}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { success = false, message = $"Server error: {errorContent}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlight(int id)
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

                var response = await httpClient.DeleteAsync($"{serverBaseUrl}/api/FlightManage/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, new { success = false, message = "Failed to delete flight" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Get Flight Details
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFlightDetails(int id)
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

                var response = await httpClient.GetAsync($"{serverBaseUrl}/api/FlightManage/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetFlightDetails: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Error communicating with server" });
            }
        }

        // Get Available Services
        [HttpGet("services")]
        public async Task<IActionResult> GetAvailableServices()
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

                var response = await httpClient.GetAsync($"{serverBaseUrl}/api/FlightManage/services");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAvailableServices: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Error communicating with server" });
            }
        }

        // Get Flight Services
        [HttpGet("{flightId}/services")]
        public async Task<IActionResult> GetFlightServices(int flightId)
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

                var response = await httpClient.GetAsync($"{serverBaseUrl}/api/FlightManage/{flightId}/services");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetFlightServices: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Error communicating with server" });
            }
        }

        // Get Flight Seats
        [HttpGet("{flightId}/seats")]
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

                var response = await httpClient.GetAsync($"{serverBaseUrl}/api/FlightManage/{flightId}/seats");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetFlightSeats: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Error communicating with server" });
            }
        }

        // Add Service to Flight
        [HttpPost("{flightId}/services")]
        public async Task<IActionResult> AddServiceToFlight(int flightId, [FromBody] AddServiceRequest request)
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

                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{serverBaseUrl}/api/FlightManage/{flightId}/services", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddServiceToFlight: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Error communicating with server" });
            }
        }

        // Remove Service from Flight
        [HttpDelete("{flightId}/services/{serviceId}")]
        public async Task<IActionResult> RemoveServiceFromFlight(int flightId, int serviceId)
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

                var response = await httpClient.DeleteAsync($"{serverBaseUrl}/api/FlightManage/{flightId}/services/{serviceId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RemoveServiceFromFlight: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Error communicating with server" });
            }
        }

        // Regenerate Flight Seats
        [HttpPost("{flightId}/seats/regenerate")]
        public async Task<IActionResult> RegenerateFlightSeats(int flightId)
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

                var response = await httpClient.PostAsync($"{serverBaseUrl}/api/FlightManage/{flightId}/seats/regenerate", null);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RegenerateFlightSeats: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Error communicating with server" });
            }
        }
    }

    // DTO classes for Flight Management API requests
    public class FlightListRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public int? StatusId { get; set; }
        public string? DepartureFrom { get; set; }
        public string? DepartureTo { get; set; }
    }

    public class FlightConflictRequest
    {
        public int? FlightId { get; set; }
        public string DepartureTime { get; set; } = null!;
        public string ArrivalTime { get; set; } = null!;
        public int PlaneId { get; set; }
        public int DepartureAirportId { get; set; }
        public int ArrivalAirportId { get; set; }
    }

    public class FlightCreateRequest
    {
        public string FlightCode { get; set; } = null!;
        public decimal Tax { get; set; }
        public string DepartureTime { get; set; } = null!;
        public string ArrivalTime { get; set; } = null!;
        public int DepartureAirportId { get; set; }
        public int ArrivalAirportId { get; set; }
        public int PlaneId { get; set; }
        public int CustomerId { get; set; }
        public int StatusId { get; set; }
    }

    public class FlightUpdateRequest
    {
        public int FlightId { get; set; }
        public string FlightCode { get; set; } = null!;
        public decimal Tax { get; set; }
        public string DepartureTime { get; set; } = null!;
        public string ArrivalTime { get; set; } = null!;
        public int DepartureAirportId { get; set; }
        public int ArrivalAirportId { get; set; }
        public int PlaneId { get; set; }
        public int CustomerId { get; set; }
        public int StatusId { get; set; }
    }

    public class AddServiceRequest
    {
        public int ServiceId { get; set; }
    }

    public class FlightSeatUpdateRequest
    {
        public int SeatId { get; set; }
        public bool IsSat { get; set; }
        public int? TicketId { get; set; }
    }
}
