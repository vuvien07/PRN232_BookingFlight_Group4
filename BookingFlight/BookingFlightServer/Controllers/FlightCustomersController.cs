using BookingFlightServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FlightCustomersController : ControllerBase
    {
        private readonly BookingFlightContext _context;

        public FlightCustomersController(BookingFlightContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var customers = await _context.Customers
                    .Include(c => c.Account)
                    // Remove StatusId filter to get ALL customers  
                    .Select(c => new
                    {
                        c.CustomerId,
                        c.Fullname,
                        c.Email,
                        c.PhoneNumber,
                        c.AccountId,
                        AccountStatus = c.Account != null ? c.Account.StatusId : 0,
                        StatusName = c.Account != null && c.Account.StatusId == 1 ? "Active" : "Inactive"
                    })
                    .OrderBy(c => c.Fullname)
                    .ToListAsync();

                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving customers", error = ex.Message });
            }
        }

        [HttpGet("by-account/{accountId}")]
        public async Task<IActionResult> GetCustomerByAccountId(int accountId)
        {
            try
            {
                var customer = await _context.Customers
                    .Include(c => c.Account)
                    .Where(c => c.AccountId == accountId)
                    .Select(c => new
                    {
                        c.CustomerId,
                        c.Fullname,
                        c.Email,
                        c.PhoneNumber,
                        c.AccountId,
                        AccountStatus = c.Account != null ? c.Account.StatusId : 0,
                        StatusName = c.Account != null && c.Account.StatusId == 1 ? "Active" : "Inactive"
                    })
                    .FirstOrDefaultAsync();

                if (customer == null)
                {
                    return NotFound(new { message = "Customer not found for this account" });
                }

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving customer", error = ex.Message });
            }
        }
    }
}
