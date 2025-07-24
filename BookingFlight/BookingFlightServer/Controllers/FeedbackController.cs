using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookingFlightServer.Services;
using BookingFlightServer.DTO.Request;
using System.Security.Claims;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackDTO createFeedbackDTO)
        {
            try
            {
                // Log the incoming request
                Console.WriteLine($"[DEBUG] Received feedback request: Title={createFeedbackDTO?.Title}, Rate={createFeedbackDTO?.Rate}, Content Length={createFeedbackDTO?.Content?.Length}, TicketId={createFeedbackDTO?.TicketId}");
                
                var accountIdClaim = User.FindFirst("AccountId")?.Value;
                Console.WriteLine($"[DEBUG] AccountId claim: {accountIdClaim}");
                
                if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
                {
                    Console.WriteLine($"[DEBUG] Invalid user token - AccountId claim is null or invalid");
                    return Unauthorized("Invalid user token");
                }

                Console.WriteLine($"[DEBUG] Parsed AccountId: {accountId}");
                
                var result = await _feedbackService.CreateFeedbackAsync(createFeedbackDTO, accountId);
                Console.WriteLine($"[DEBUG] CreateFeedbackAsync result: {result}");
                
                if (result != null)
                {
                    return Ok(new { 
                        success = true,
                        message = "Feedback đã được gửi thành công và lưu vào cơ sở dữ liệu!",
                        data = result
                    });
                }
                return BadRequest(new { 
                    success = false,
                    message = "Không thể tạo feedback. Vui lòng thử lại." 
                });
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[DEBUG] InvalidOperationException: {ex.Message}");
                return BadRequest(new { 
                    success = false,
                    message = ex.Message 
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Exception: {ex.Message}");
                Console.WriteLine($"[DEBUG] Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { 
                    success = false,
                    message = "Lỗi hệ thống", 
                    error = ex.Message 
                });
            }
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            try
            {
                var feedbacks = await _feedbackService.GetAllFeedbacksAsync();
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("my-feedbacks")]
        public async Task<IActionResult> GetMyFeedbacks()
        {
            try
            {
                var accountIdClaim = User.FindFirst("AccountId")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
                {
                    return Unauthorized("Invalid user token");
                }

                var feedbacks = await _feedbackService.GetFeedbacksByAccountIdAsync(accountId);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("by-ticket/{ticketId}")]
        public async Task<IActionResult> GetFeedbacksByTicketId(int ticketId)
        {
            try
            {
                var feedbacks = await _feedbackService.GetFeedbacksByTicketIdAsync(ticketId);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("detail/all-by-ticket/{ticketId}")]
        public async Task<IActionResult> GetFeedbacksDetailByTicketId(int ticketId)
        {
            try
            {
                Console.WriteLine($"[DEBUG] GetFeedbacksDetailByTicketId called with TicketId: {ticketId}");
                
                var feedbackDetails = await _feedbackService.GetFeedbacksDetailByTicketIdAsync(ticketId);
                
                Console.WriteLine($"[DEBUG] GetFeedbacksDetailByTicketId returned {feedbackDetails.Count} feedback(s)");
                if (feedbackDetails.Any())
                {
                    Console.WriteLine($"[DEBUG] First feedback - FeedbackId: {feedbackDetails[0].FeedbackId}, AccountId: {feedbackDetails[0].AccountId}");
                }
                
                return Ok(feedbackDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Exception in GetFeedbacksDetailByTicketId: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("detail/my-feedbacks")]
        public async Task<IActionResult> GetMyFeedbacksDetail()
        {
            try
            {
                var accountIdClaim = User.FindFirst("AccountId")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
                {
                    return Unauthorized("Invalid user token");
                }

                var feedbackDetails = await _feedbackService.GetFeedbacksDetailByAccountIdAsync(accountId);
                return Ok(feedbackDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("detail/all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllFeedbacksDetail()
        {
            try
            {
                var feedbackDetails = await _feedbackService.GetAllFeedbackDetailsAsync();
                return Ok(feedbackDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchAndFilterFeedbacks(
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? rating = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var feedbackDetails = await _feedbackService.SearchAndFilterFeedbacksAsync(searchTerm, rating, fromDate, toDate);
                return Ok(feedbackDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("check/ticket/{ticketId}")]
        public async Task<IActionResult> CheckFeedbackByTicketId(int ticketId)
        {
            try
            {
                var accountIdClaim = User.FindFirst("AccountId")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
                {
                    return Unauthorized("Invalid user token");
                }

                var hasFeedback = await _feedbackService.HasFeedbackByTicketIdAsync(ticketId, accountId);
                return Ok(new { hasFeedback });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("detail/by-ticket/{ticketId}")]
        public async Task<IActionResult> GetFeedbackDetailsByTicketId(int ticketId, [FromQuery] int accountId)
        {
            try
            {
                var accountIdClaim = User.FindFirst("AccountId")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int jwtAccountId))
                {
                    return Unauthorized("Invalid user token");
                }

                Console.WriteLine($"[DEBUG] TicketId: {ticketId}, JWT AccountId: {jwtAccountId}, Query AccountId: {accountId}");

                // Get feedback for this ticket filtered by the current user (from JWT)
                var feedbackDetails = await _feedbackService.GetFeedbackDetailsByTicketIdAsync(ticketId, jwtAccountId);
                
                Console.WriteLine($"[DEBUG] Found {feedbackDetails.Count} feedback(s) for TicketId {ticketId} and AccountId {jwtAccountId}");
                if (feedbackDetails.Any())
                {
                    Console.WriteLine($"[DEBUG] First feedback - FeedbackId: {feedbackDetails[0].FeedbackId}, AccountId: {feedbackDetails[0].AccountId}");
                }

                return Ok(feedbackDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Exception in GetFeedbackDetailsByTicketId: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
