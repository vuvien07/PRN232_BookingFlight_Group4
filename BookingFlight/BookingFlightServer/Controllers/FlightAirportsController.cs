using BookingFlightServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FlightAirportsController : ControllerBase
    {
        private readonly BookingFlightContext _context;

        public FlightAirportsController(BookingFlightContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAirports()
        {
            try
            {
                var airports = await _context.Airports
                    .Select(a => new
                    {
                        a.AirportId,
                        a.AirportName,
                        a.AirportCode,
                        Location = a.City
                    })
                    .OrderBy(a => a.AirportName)
                    .ToListAsync();

                return Ok(airports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving airports", error = ex.Message });
            }
        }
    }
}
