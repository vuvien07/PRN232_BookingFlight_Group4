using BookingFlightServer.DTO.Request;
using BookingFlightServer.Services;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace BookingFlightServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailVerificationController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public EmailVerificationController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Verify email address using token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();
                    return BadRequest(new { success = false, message = "Validation failed", errors });
                }

                var result = await _accountService.VerifyEmailAsync(request.Token);
                
                if (result)
                {
                    return Ok(new 
                    { 
                        success = true, 
                        message = "Email verified successfully! Your account is now active and you can login." 
                    });
                }
                else
                {
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "Invalid or expired verification token." 
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = "An error occurred while verifying your email.",
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Validate verification token (check if token is valid and not expired)
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("validate-token")]
        public async Task<IActionResult> ValidateVerificationToken([FromQuery] string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { success = false, message = "Token is required." });
                }                // Get repository directly for validation
                var accountRepository = HttpContext.RequestServices.GetService<IAccountRepository>();
                (int? accountId, DateTime? expireTime) = await accountRepository.GetEmailVerificationTokenInfoAsync(token);

                if (accountId == null || expireTime == null)
                {
                    return BadRequest(new { success = false, message = "Invalid verification token." });
                }

                if (DateTime.Now > expireTime)
                {
                    return BadRequest(new { success = false, message = "Verification token has expired." });
                }

                return Ok(new { success = true, message = "Token is valid." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = "An error occurred while validating token.",
                    error = ex.Message 
                });
            }
        }
    }
}
