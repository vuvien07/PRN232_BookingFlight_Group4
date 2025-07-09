using BookingFlightServer.Services;
using BookingFlightServer.Utils;
using Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.RoleAdmin)]
    public class ManageAccountController : ControllerBase
    {
        private readonly IManageAccountService manageAccountService;

        public ManageAccountController(IManageAccountService manageAccountService)
        {
            this.manageAccountService = manageAccountService;
        }
        /// <summary>
        /// Function View list account (Manage Account)
        /// </summary>
        /// <returns>List of ResponseAccountDTO</returns>
        // GET: api/manageaccount/get-all-account
        [HttpGet]
        [Route("get-all-account")]
        public async Task<IActionResult> GetAllAcounts()
        {
            // Call the service to get the list of accounts
            var responseAccountDTO = await manageAccountService.GetAccountsAsync();

            // Check if the response is null or empty
            if (responseAccountDTO == null)
            {
                return NotFound(new { message = "No accounts found." });
            }
            // Return the list of accounts
            return Ok(responseAccountDTO);
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        /// <returns>ResponseAccountDTO</returns>
        // GET: api/manageaccount/get-my-profile
        [HttpGet]
        [Route("get-my-profile")]
        [Authorize] // Allow any authenticated user to get their own profile
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
    }
}
