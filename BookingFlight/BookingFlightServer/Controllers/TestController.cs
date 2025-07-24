using BookingFlightServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IGeminiAIService _geminiAiService;
        private readonly IComplaintService _complaintService;

        public TestController(IGeminiAIService geminiAiService, IComplaintService complaintService)
        {
            _geminiAiService = geminiAiService;
            _complaintService = complaintService;
        }

        [HttpPost("test-complaint-ai")]
        public async Task<IActionResult> TestComplaintAI([FromBody] TestComplaintRequest request)
        {
            try
            {
                var isRelevant = await _geminiAiService.IsComplaintRelevantAsync(request.ComplaintDescription);
                var rejectionReason = await _geminiAiService.GenerateRejectionReasonAsync(request.ComplaintDescription);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        complaintDescription = request.ComplaintDescription,
                        isRelevant = isRelevant,
                        rejectionReason = rejectionReason,
                        status = isRelevant ? "Will remain Pending for supporter review" : "Will be automatically rejected after 1 minute",
                        message = isRelevant ? "✅ AI coi khiếu nại này là LIÊN QUAN - sẽ để supporter xử lý" : "❌ AI coi khiếu nại này là KHÔNG LIÊN QUAN - sẽ tự động reject"
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("pending-complaints")]
        public async Task<IActionResult> GetPendingComplaints()
        {
            try
            {
                var pendingComplaints = await _complaintService.GetComplaintsByStatusAsync(3); // Status 3 = Pending
                var oneMinuteAgo = DateTime.Now.AddMinutes(-1);

                var complaintsToProcess = pendingComplaints
                    .Where(c => c.CreateAt.HasValue && c.CreateAt.Value <= oneMinuteAgo)
                    .Select(c => new
                    {
                        c.ComplaintId,
                        c.Description,
                        c.CreateAt,
                        c.CustomerName,
                        c.CustomerEmail,
                        ReadyForProcessing = c.CreateAt.HasValue && c.CreateAt.Value <= oneMinuteAgo
                    })
                    .ToList();

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        totalPendingComplaints = pendingComplaints.Count(),
                        complaintsReadyForProcessing = complaintsToProcess.Count,
                        complaints = complaintsToProcess
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("toggle-background-service")]
        public IActionResult ToggleBackgroundService([FromBody] ToggleBackgroundServiceRequest request)
        {
            try
            {
                BookingFlightServer.Services.Implements.ComplaintProcessingBackgroundService.SetEnabled(request.Enabled);
                return Ok(new 
                { 
                    success = true, 
                    message = request.Enabled ? "Background service enabled" : "Background service disabled",
                    isEnabled = BookingFlightServer.Services.Implements.ComplaintProcessingBackgroundService.IsEnabled
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("background-service-status")]
        public IActionResult GetBackgroundServiceStatus()
        {
            return Ok(new 
            { 
                success = true, 
                isEnabled = BookingFlightServer.Services.Implements.ComplaintProcessingBackgroundService.IsEnabled,
                message = BookingFlightServer.Services.Implements.ComplaintProcessingBackgroundService.IsEnabled ? "Service is running" : "Service is disabled"
            });
        }
    }

    public class TestComplaintRequest
    {
        public string ComplaintDescription { get; set; } = string.Empty;
    }

    public class ToggleBackgroundServiceRequest
    {
        public bool Enabled { get; set; }
    }
}
