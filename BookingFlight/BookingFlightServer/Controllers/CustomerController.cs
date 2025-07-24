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
    public class CustomerController : ControllerBase
    {
        private readonly BookingFlightContext _context;

        public CustomerController(BookingFlightContext context)
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

                var customer = await _context.Customers
                    .Include(c => c.Account)
                    .FirstOrDefaultAsync(c => c.Account.Username == username);

                if (customer == null)
                {
                    return NotFound(new { message = "Customer not found" });
                }

                return Ok(new
                {
                    CustomerId = customer.CustomerId,
                    Username = customer.Account.Username,
                    Fullname = customer.Fullname,
                    Email = customer.Email
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

                var customer = await _context.Customers
                    .Include(c => c.Account)
                    .FirstOrDefaultAsync(c => c.Account.Username == username);

                if (customer == null)
                {
                    return NotFound(new { message = "Customer not found" });
                }

                return Ok(new
                {
                    CustomerId = customer.CustomerId,
                    Username = customer.Account.Username,
                    Fullname = customer.Fullname,
                    Email = customer.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
