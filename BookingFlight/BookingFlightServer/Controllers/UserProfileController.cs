using BookingFlightServer.DTO.ManageAccount;
using BookingFlightServer.Services;
using Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Allow any authenticated user
    public class UserProfileController : ControllerBase
    {
        private readonly IManageAccountService manageAccountService;
        private readonly ILogger<UserProfileController> logger;

        public UserProfileController(IManageAccountService manageAccountService, ILogger<UserProfileController> logger)
        {
            this.manageAccountService = manageAccountService;
            this.logger = logger;
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        /// <returns>ResponseAccountDTO</returns>
        // GET: api/userprofile/me
        [HttpGet]
        [Route("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                // Get token from header
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "No token provided" });
                }

                // Get username from token
                var username = JwtDecoder.GetUsernameFromToken(token);
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                // Get all accounts and find the current user
                var accounts = await manageAccountService.GetAccountsAsync();
                var userAccount = accounts?.FirstOrDefault(a => a.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

                if (userAccount == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(userAccount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Update current user profile
        /// </summary>
        /// <param name="updateRequest">Updated user data</param>
        /// <returns>Success/failure response</returns>
        // PUT: api/userprofile/me
        [HttpPut]
        [Route("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] ResponseAccountDTO updateRequest)
        {
            try
            {
                logger.LogInformation("UpdateMyProfile called");
                logger.LogInformation("Update request data: {Data}", JsonSerializer.Serialize(updateRequest));

                // Get token from header
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(token))
                {
                    logger.LogWarning("No token provided");
                    return Unauthorized(new { message = "No token provided" });
                }

                // Get username from token
                var username = JwtDecoder.GetUsernameFromToken(token);
                if (string.IsNullOrEmpty(username))
                {
                    logger.LogWarning("Invalid token");
                    return Unauthorized(new { message = "Invalid token" });
                }

                logger.LogInformation("Updating profile for user: {Username}", username);

                // Update user profile
                var result = await manageAccountService.UpdateAccountAsync(username, updateRequest);
                logger.LogInformation("Update result: {Result}", result);

                if (result)
                {
                    return Ok(new { message = "Profile updated successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Failed to update profile" });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating profile");
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }
    }
}
