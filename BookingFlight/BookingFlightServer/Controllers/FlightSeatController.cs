using BookingFlightServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightSeatController : ControllerBase
    {
        private readonly FlightSeatService _flightSeatService;
        private readonly IFlightService _flightService;

        public FlightSeatController(FlightSeatService flightSeatService, IFlightService flightService)
        {
            _flightSeatService = flightSeatService;
            _flightService = flightService;
        }

        [HttpGet("flights")]
        public async Task<IActionResult> GetAllFlights()
        {
            try
            {
                var flights = await _flightService.GetAllFlights();
                return Ok(new { success = true, data = flights });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{flightId}/seats")]
        public async Task<IActionResult> GetFlightSeats(int flightId)
        {
            try
            {
                var seats = await _flightSeatService.GetFlightSeatsAsync(flightId);
                return Ok(new { success = true, data = seats });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{flightId}/seats/{seatId}")]
        public async Task<IActionResult> GetFlightSeat(int flightId, int seatId)
        {
            try
            {
                var seat = await _flightSeatService.GetFlightSeatByIdAsync(flightId, seatId);
                if (seat == null)
                {
                    return NotFound(new { success = false, message = "Seat not found" });
                }
                return Ok(new { success = true, data = seat });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{flightId}/seats/available")]
        public async Task<IActionResult> GetAvailableSeats(int flightId)
        {
            try
            {
                var seats = await _flightSeatService.GetAvailableSeatsAsync(flightId);
                return Ok(new { success = true, data = seats });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{flightId}/seats/occupied")]
        public async Task<IActionResult> GetOccupiedSeats(int flightId)
        {
            try
            {
                var seats = await _flightSeatService.GetOccupiedSeatsAsync(flightId);
                return Ok(new { success = true, data = seats });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{flightId}/seats/statistics")]
        public async Task<IActionResult> GetSeatStatistics(int flightId)
        {
            try
            {
                var (available, occupied) = await _flightSeatService.GetSeatStatisticsAsync(flightId);
                return Ok(new 
                { 
                    success = true, 
                    data = new 
                    { 
                        flightId, 
                        availableSeats = available, 
                        occupiedSeats = occupied, 
                        totalSeats = available + occupied 
                    } 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{flightId}/seats/mock")]
        public async Task<IActionResult> GetMockSeats(int flightId)
        {
            try
            {
                var seats = await _flightSeatService.GenerateMockSeatsAsync(flightId);
                return Ok(new { success = true, data = seats });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{flightId}/seats/create")]
        public async Task<IActionResult> CreateSeatsForFlight(int flightId)
        {
            try
            {
                var result = await _flightSeatService.CreateSeatsForFlightAsync(flightId);
                if (result)
                {
                    return Ok(new { success = true, message = "Seats created successfully" });
                }
                return BadRequest(new { success = false, message = "Failed to create seats. Seats may already exist or flight not found." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{flightId}/seats/{seatId}/assign")]
        public async Task<IActionResult> AssignSeat(int flightId, int seatId, [FromBody] AssignSeatRequest request)
        {
            try
            {
                var result = await _flightSeatService.AssignSeatToTicketAsync(flightId, seatId, request.TicketId);
                if (result)
                {
                    return Ok(new { success = true, message = "Seat assigned successfully" });
                }
                return BadRequest(new { success = false, message = "Failed to assign seat" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{flightId}/seats/{seatId}/unassign")]
        public async Task<IActionResult> UnassignSeat(int flightId, int seatId)
        {
            try
            {
                var result = await _flightSeatService.UnassignSeatAsync(flightId, seatId);
                if (result)
                {
                    return Ok(new { success = true, message = "Seat unassigned successfully" });
                }
                return BadRequest(new { success = false, message = "Failed to unassign seat" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{flightId}/seats/{seatId}/status")]
        public async Task<IActionResult> UpdateSeatStatus(int flightId, int seatId, [FromBody] UpdateSeatStatusRequest request)
        {
            try
            {
                var result = await _flightSeatService.UpdateFlightSeatStatusAsync(flightId, seatId, request.IsSat, request.TicketId);
                if (result)
                {
                    return Ok(new { success = true, message = "Seat status updated successfully" });
                }
                return BadRequest(new { success = false, message = "Failed to update seat status" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{flightId}/seats/bulk-assign")]
        public async Task<IActionResult> BulkAssignSeats(int flightId, [FromBody] BulkAssignSeatsRequest request)
        {
            try
            {
                var assignments = request.Assignments.Select(a => (a.SeatId, a.TicketId)).ToList();
                var result = await _flightSeatService.BulkAssignSeatsAsync(flightId, assignments);
                if (result)
                {
                    return Ok(new { success = true, message = "Seats assigned successfully" });
                }
                return BadRequest(new { success = false, message = "Failed to assign seats" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{flightId}/seats/bulk-unassign")]
        public async Task<IActionResult> BulkUnassignSeats(int flightId, [FromBody] BulkUnassignSeatsRequest request)
        {
            try
            {
                var result = await _flightSeatService.BulkUnassignSeatsAsync(flightId, request.SeatIds);
                if (result)
                {
                    return Ok(new { success = true, message = "Seats unassigned successfully" });
                }
                return BadRequest(new { success = false, message = "Failed to unassign seats" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    // Request models
    public class AssignSeatRequest
    {
        public int TicketId { get; set; }
    }

    public class UpdateSeatStatusRequest
    {
        public bool IsSat { get; set; }
        public int? TicketId { get; set; }
    }

    public class BulkAssignSeatsRequest
    {
        public List<SeatAssignment> Assignments { get; set; } = new();
    }

    public class SeatAssignment
    {
        public int SeatId { get; set; }
        public int TicketId { get; set; }
    }

    public class BulkUnassignSeatsRequest
    {
        public List<int> SeatIds { get; set; } = new();
    }
}
