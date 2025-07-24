using BookingFlightServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SupporterController : ControllerBase
    {
        private readonly BookingFlightContext _context;

        public SupporterController(BookingFlightContext context)
        {
            _context = context;
        }

        [HttpGet("GetByUsername/{username}")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest(new { message = "Username is required" });
                }

                var supporter = await _context.Supporters
                    .Include(s => s.Account)
                    .FirstOrDefaultAsync(s => s.Account.Username == username);

                if (supporter == null)
                {
                    return NotFound(new { message = "Supporter not found" });
                }

                return Ok(new
                {
                    SupporterId = supporter.SupporterId,
                    Username = supporter.Account.Username,
                    Fullname = supporter.Fullname,
                    Email = supporter.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("GetCurrent")]
        public async Task<IActionResult> GetCurrent()
        {
            try
            {
                // Get username from JWT token claims
                var username = User.FindFirst(ClaimTypes.Name)?.Value ?? 
                              User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { message = "Username not found in token" });
                }

                var supporter = await _context.Supporters
                    .Include(s => s.Account)
                    .FirstOrDefaultAsync(s => s.Account.Username == username);

                if (supporter == null)
                {
                    return NotFound(new { message = "Supporter not found" });
                }

                return Ok(new
                {
                    SupporterId = supporter.SupporterId,
                    Username = supporter.Account.Username,
                    Fullname = supporter.Fullname,
                    Email = supporter.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("by-account/{accountId}")]
        [AllowAnonymous] // Allow anonymous for middleware usage
        public async Task<IActionResult> GetByAccountId(int accountId)
        {
            try
            {
                var supporter = await _context.Supporters
                    .Include(s => s.Account)
                    .FirstOrDefaultAsync(s => s.AccountId == accountId);

                if (supporter == null)
                {
                    return NotFound(new { message = "Supporter not found" });
                }

                return Ok(new
                {
                    SupporterId = supporter.SupporterId,
                    AccountId = supporter.AccountId,
                    Fullname = supporter.Fullname,
                    Email = supporter.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
