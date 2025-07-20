using BookingFlightServer.DTO.Manager;
using BookingFlightServer.Services;
using BookingFlightServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager")]
    public class FlightManageController : ControllerBase
    {
        private readonly IFlightManageService _flightManageService;
        private readonly BookingFlightContext _context;
        private readonly ILogger<FlightManageController> _logger;

        public FlightManageController(IFlightManageService flightManageService, BookingFlightContext context, ILogger<FlightManageController> logger)
        {
            _flightManageService = flightManageService;
            _context = context;
            _logger = logger;
        }

        private async Task<int> GetManagerIdAsync()
        {
            var usernameClaim = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(usernameClaim))
            {
                throw new UnauthorizedAccessException("Username not found in token");
            }

            var manager = await _context.Managers
                .Include(m => m.Account)
                .FirstOrDefaultAsync(m => m.Account.Username == usernameClaim);

            if (manager == null)
            {
                // If no manager found, create a default manager for development/testing
                // In production, this should be handled differently
                _logger.LogWarning($"No manager found for username: {usernameClaim}. Using default manager ID 1 for development.");
                
                // Check if manager with ID 1 exists
                var defaultManager = await _context.Managers.FirstOrDefaultAsync(m => m.ManagerId == 1);
                if (defaultManager != null)
                {
                    return 1; // Return default manager ID
                }
                
                throw new UnauthorizedAccessException($"Manager not found for username: {usernameClaim}");
            }

            return manager.ManagerId;
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetFlights([FromBody] FlightListRequestDTO request)
        {
            try
            {
                // Only show flights for the logged-in manager
                request.ManagerId = await GetManagerIdAsync();
                
                var flights = await _flightManageService.GetFlightsByFilter(request);
                var totalCount = await _flightManageService.GetTotalFlightsCount(request);
                var totalPages = Math.Ceiling((double)totalCount / request.PageSize);

                return Ok(new
                {
                    success = true,
                    data = flights,
                    pagination = new
                    {
                        currentPage = request.Page,
                        pageSize = request.PageSize,
                        totalCount,
                        totalPages
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting flights list");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFlight(int id)
        {
            try
            {
                var flight = await _flightManageService.GetFlightById(id);
                if (flight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }

                // Check if manager can access this flight
                var managerId = await GetManagerIdAsync();
                if (flight.ManagerId != managerId)
                {
                    return Forbid();
                }

                return Ok(new { success = true, data = flight });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting flight by ID: {FlightId}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlight([FromBody] FlightCreateRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Validation failed", errors = ModelState });
                }

                var managerId = await GetManagerIdAsync();
                var flight = await _flightManageService.CreateFlight(request, managerId);

                return Ok(new { success = true, data = flight, message = "Flight created successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating flight");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFlight(int id, [FromBody] FlightUpdateRequestDTO request)
        {
            try
            {
                if (id != request.FlightId)
                {
                    return BadRequest(new { success = false, message = "Flight ID mismatch" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Validation failed", errors = ModelState });
                }

                // Check if flight exists and get departure time
                var existingFlight = await _context.Flights
                    .FirstOrDefaultAsync(f => f.FlightId == id);

                if (existingFlight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }

                // Check if departure time is in the past
                if (existingFlight.DepartureTime <= DateTime.Now)
                {
                    return BadRequest(new { success = false, message = "Cannot update flights that have already departed or are departing now" });
                }

                var managerId = await GetManagerIdAsync();
                var flight = await _flightManageService.UpdateFlight(request, managerId);

                return Ok(new { success = true, data = flight, message = "Flight updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating flight");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            try
            {
                // Check if flight exists and get departure time
                var existingFlight = await _context.Flights
                    .FirstOrDefaultAsync(f => f.FlightId == id);

                if (existingFlight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }

                // Check if departure time is in the past
                if (existingFlight.DepartureTime <= DateTime.Now)
                {
                    return BadRequest(new { success = false, message = "Cannot delete flights that have already departed or are departing now" });
                }

                var flight = await _flightManageService.GetFlightById(id);
                if (flight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }

                // Check if manager can delete this flight
                var managerId = await GetManagerIdAsync();
                if (flight.ManagerId != managerId)
                {
                    return Forbid();
                }

                var result = await _flightManageService.DeleteFlight(id);
                if (result)
                {
                    return Ok(new { success = true, message = "Flight deleted successfully" });
                }

                return BadRequest(new { success = false, message = "Failed to delete flight" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting flight");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("check-conflicts")]
        public async Task<IActionResult> CheckFlightConflicts([FromBody] FlightConflictCheckRequestDTO request)
        {
            try
            {
                var result = await _flightManageService.CheckFlightConflicts(request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking flight conflicts");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetFlightStatuses()
        {
            try
            {
                var statuses = await _flightManageService.GetFlightStatuses();
                return Ok(new { success = true, data = statuses });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting flight statuses");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("my-flights")]
        public async Task<IActionResult> GetMyFlights()
        {
            try
            {
                var managerId = await GetManagerIdAsync();
                var flights = await _flightManageService.GetFlightsByManagerId(managerId);
                return Ok(new { success = true, data = flights });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting manager flights");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}