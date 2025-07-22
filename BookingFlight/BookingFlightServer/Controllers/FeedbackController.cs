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
                Console.WriteLine($"[DEBUG] Received feedback request: Title={createFeedbackDTO?.Title}, Rate={createFeedbackDTO?.Rate}, Content Length={createFeedbackDTO?.Content?.Length}");
                
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
                
                if (result)
                {
                    return Ok(new { 
                        success = true,
                        message = "Feedback đã được gửi thành công và lưu vào cơ sở dữ liệu!",
                        data = new {
                            accountId = accountId,
                            title = createFeedbackDTO.Title,
                            rate = createFeedbackDTO.Rate,
                            createdAt = DateTime.Now
                        }
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
    }
}
