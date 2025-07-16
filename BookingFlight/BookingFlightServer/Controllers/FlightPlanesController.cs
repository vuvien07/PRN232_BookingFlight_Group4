using BookingFlightServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FlightPlanesController : ControllerBase
    {
        private readonly BookingFlightContext _context;

        public FlightPlanesController(BookingFlightContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPlanes()
        {
            try
            {
                var planes = await _context.Planes
                    .Where(p => p.StatusId == 1) // Active planes
                    .Include(p => p.Seats)
                    .Select(p => new
                    {
                        p.PlaneId,
                        p.PlaneCode,
                        PlaneName = p.Model,
                        p.Manufacture,
                        TotalSeat = p.Seats.Count()
                    })
                    .OrderBy(p => p.PlaneCode)
                    .ToListAsync();

                return Ok(planes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving planes", error = ex.Message });
            }
        }
    }
}
