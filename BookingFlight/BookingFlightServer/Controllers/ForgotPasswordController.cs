using BookingFlightServer.DTO.Request;
using BookingFlightServer.Services;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace BookingFlightServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public ForgotPasswordController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Send forgot password email
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("send-email")]
        public async Task<IActionResult> SendForgotPasswordEmail([FromBody] ForgotPasswordDTO request)
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

                var result = await _accountService.SendForgotPasswordEmailAsync(request.Email);
                
                if (result)
                {
                    return Ok(new 
                    { 
                        success = true, 
                        message = "Reset password instructions have been sent to your email." 
                    });
                }
                else
                {
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "Email not found in our system." 
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = "An error occurred while processing your request.",
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Reset password using token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO request)
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

                var result = await _accountService.ResetPasswordAsync(request.Token, request.NewPassword);
                
                if (result)
                {
                    return Ok(new 
                    { 
                        success = true, 
                        message = "Password has been reset successfully." 
                    });
                }
                else
                {
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "Invalid or expired reset token." 
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = "An error occurred while processing your request.",
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Validate reset token (check if token is valid and not expired)
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("validate-token")]
        public async Task<IActionResult> ValidateToken([FromQuery] string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { success = false, message = "Token is required." });
                }

                // We can use the repository directly for this simple check
                // In a real app, you might want to add this to the service layer
                var accountRepository = HttpContext.RequestServices.GetService<IAccountRepository>();
                var (accountId, expireTime) = await accountRepository.GetResetTokenInfoAsync(token);

                if (accountId == null || expireTime == null)
                {
                    return BadRequest(new { success = false, message = "Invalid token." });
                }

                if (DateTime.Now > expireTime)
                {
                    return BadRequest(new { success = false, message = "Token has expired." });
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
