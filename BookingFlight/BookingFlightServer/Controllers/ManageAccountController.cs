using BookingFlightServer.Services;
using BookingFlightServer.Utils;
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
    }
}
