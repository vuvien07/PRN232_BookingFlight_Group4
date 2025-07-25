using Microsoft.AspNetCore.Mvc;
using BookingFlightServer.Services.Interfaces;
using BookingFlightServer.DTO.Response;

namespace BookingFlightServer.Controllers
{
    [ApiController]
    [Route("api/supporter-complaint")]
    public class SupporterComplaintController : ControllerBase
    {
        private readonly ISupporterComplaintService _supporterComplaintService;
        private readonly ILogger<SupporterComplaintController> _logger;

        public SupporterComplaintController(
            ISupporterComplaintService supporterComplaintService,
            ILogger<SupporterComplaintController> logger)
        {
            _supporterComplaintService = supporterComplaintService;
            _logger = logger;
        }

        [HttpGet("supporter-complaints")]
        public async Task<IActionResult> GetSupporterComplaints()
        {
            try
            {
                _logger.LogInformation("Getting supporter complaints");
                var complaints = await _supporterComplaintService.GetComplaintsForSupporterAsync();
                _logger.LogInformation($"Found {complaints.Count()} supporter complaints");
                return Ok(complaints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting supporter complaints");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("debug-raw")]
        public async Task<IActionResult> GetRawComplaints()
        {
            try
            {
                _logger.LogInformation("Getting raw complaints for debugging");
                var complaints = await _supporterComplaintService.GetComplaintsForSupporterAsync();
                _logger.LogInformation($"Raw complaints count: {complaints.Count()}");
                
                var result = complaints.Select(c => new {
                    ComplaintId = c.ComplaintId,
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    StatusId = c.StatusId,
                    StatusName = c.StatusName,
                    Description = c.Description,
                    CreateAt = c.CreateAt
                });
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting raw complaints");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetComplaintStatistics()
        {
            try
            {
                var statistics = await _supporterComplaintService.GetComplaintStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting complaint statistics");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{complaintId}/resolve")]
        public async Task<IActionResult> ResolveComplaint(int complaintId)
        {
            try
            {
                var success = await _supporterComplaintService.UpdateComplaintStatusAsync(complaintId, 4); // Status 4 = Resolved
                if (success)
                {
                    return Ok(new { message = "Complaint resolved successfully" });
                }
                return BadRequest(new { message = "Failed to resolve complaint" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving complaint {ComplaintId}", complaintId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("{complaintId}/assign/{supporterId}")]
        public async Task<IActionResult> AssignComplaint(int complaintId, int supporterId)
        {
            try
            {
                var success = await _supporterComplaintService.AssignComplaintToSupporterAsync(complaintId, supporterId);
                if (success)
                {
                    return Ok(new { message = "Complaint assigned successfully" });
                }
                return BadRequest(new { message = "Failed to assign complaint" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning complaint {ComplaintId} to supporter {SupporterId}", complaintId, supporterId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
