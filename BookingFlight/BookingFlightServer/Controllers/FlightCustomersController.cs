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
                    .Where(c => c.Account.StatusId == 1) // Active customers
                    .Select(c => new
                    {
                        c.CustomerId,
                        c.Fullname,
                        c.Email,
                        c.PhoneNumber
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
    }
}
