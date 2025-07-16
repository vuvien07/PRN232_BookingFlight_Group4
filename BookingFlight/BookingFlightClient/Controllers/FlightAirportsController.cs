using Microsoft.AspNetCore.Mvc;

namespace BookingFlightClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightAirportsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public FlightAirportsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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

        [HttpGet]
        public async Task<IActionResult> GetAirports()
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

                var response = await httpClient.GetAsync($"{serverBaseUrl}/api/FlightAirports");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Content(responseContent, "application/json");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, new { success = false, message = "Failed to fetch airports" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
