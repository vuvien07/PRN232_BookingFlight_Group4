using Microsoft.AspNetCore.Mvc;

namespace BookingFlightClient.Controllers
{
    public class FileController : Controller
    {
        private readonly HttpClient _httpClient;

        public FileController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("IgnoreSSL");
        }

        [HttpGet("File/Complaints/{fileName}")]
        public async Task<IActionResult> GetComplaintFile(string fileName)
        {
            try
            {
                // Try HTTP first, then HTTPS if needed
                var serverUrl = $"http://localhost:5077/api/file/complaints/{fileName}";
                var response = await _httpClient.GetAsync(serverUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    var contentType = "application/octet-stream"; // Force download for all files
                    
                    // Force download by setting Content-Disposition header
                    return File(content, contentType, fileName);
                }

                // Log the error response
                var errorContent = await response.Content.ReadAsStringAsync();
                return BadRequest($"Server error: {response.StatusCode} - {errorContent}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving file: {ex.Message}");
            }
        }
    }
}
