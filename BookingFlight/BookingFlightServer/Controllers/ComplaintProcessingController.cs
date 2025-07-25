using Microsoft.AspNetCore.Mvc;
using BookingFlightServer.Services.Implements;

namespace BookingFlightServer.Controllers
{
    [ApiController]
    [Route("api/complaint-processing")]
    public class ComplaintProcessingController : ControllerBase
    {
        private readonly ILogger<ComplaintProcessingController> _logger;

        public ComplaintProcessingController(ILogger<ComplaintProcessingController> logger)
        {
            _logger = logger;
        }

        [HttpPost("enable")]
        public IActionResult EnableProcessing()
        {
            ComplaintProcessingBackgroundService.SetEnabled(true);
            _logger.LogInformation("Complaint AI processing enabled");
            return Ok(new { message = "Complaint AI processing enabled", enabled = true });
        }

        [HttpPost("disable")]
        public IActionResult DisableProcessing()
        {
            ComplaintProcessingBackgroundService.SetEnabled(false);
            _logger.LogInformation("Complaint AI processing disabled");
            return Ok(new { message = "Complaint AI processing disabled", enabled = false });
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var isEnabled = ComplaintProcessingBackgroundService.IsEnabled;
            return Ok(new { enabled = isEnabled, message = $"Complaint AI processing is {(isEnabled ? "enabled" : "disabled")}" });
        }
    }
}
