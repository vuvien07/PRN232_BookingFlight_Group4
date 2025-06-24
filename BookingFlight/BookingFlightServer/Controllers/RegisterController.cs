using BookingFlightServer.DTO.Request;
using BookingFlightServer.Services;
using BookingFlightServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public RegisterController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "Register API is working!", timestamp = DateTime.Now });
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = UtilHelper.GetModelStateErrors(ModelState);
                    return BadRequest(new { message = "Validation failed", errors });
                }

                // Check if username already exists
                if (await _accountService.IsUsernameExistsAsync(registerDTO.Username!))
                {
                    return BadRequest(new { message = "Username already exists" });
                }

                // Check if email already exists
                if (await _accountService.IsEmailExistsAsync(registerDTO.Email!))
                {
                    return BadRequest(new { message = "Email already exists" });
                }                // Register customer
                var customer = await _accountService.RegisterCustomerAsync(
                    registerDTO.Username!,
                    registerDTO.Password!,
                    registerDTO.Fullname!,
                    registerDTO.Address!,
                    registerDTO.PhoneNumber!,
                    registerDTO.Email!
                );

                return Ok(new { 
                    success = true,
                    message = "Registration successful! Please check your email to verify your account before logging in.", 
                    customerId = customer.CustomerId,
                    requiresEmailVerification = true
                });
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Registration error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpGet("check-username/{username}")]
        public async Task<IActionResult> CheckUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { message = "Username is required" });
            }

            var exists = await _accountService.IsUsernameExistsAsync(username);
            return Ok(new { exists });
        }

        [HttpGet("check-email/{email}")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            var exists = await _accountService.IsEmailExistsAsync(email);
            return Ok(new { exists });
        }
    }
}
