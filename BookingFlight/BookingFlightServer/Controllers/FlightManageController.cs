using BookingFlightServer.DTO.Manager;
using BookingFlightServer.Services;
using BookingFlightServer.Data;
using BookingFlightServer.Entities;
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

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetFlightDetails(int id)
        {
            try
            {
                var managerId = await GetManagerIdAsync();
                
                // Get flight with all related data
                var flight = await _context.Flights
                    .Include(f => f.Plane)
                    .Include(f => f.DepartureAirport)
                    .Include(f => f.ArrivalAirport)
                    .Include(f => f.Status)
                    .Include(f => f.Manager)
                    .FirstOrDefaultAsync(f => f.FlightId == id);

                if (flight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }

                // Check if manager can access this flight
                if (flight.ManagerId != managerId)
                {
                    return Forbid();
                }

                // Get flight seats with details
                var flightSeats = await _context.FlightSeats
                    .Where(fs => fs.FlightId == id)
                    .Include(fs => fs.Seat)
                        .ThenInclude(s => s.Class)
                    .Include(fs => fs.Seat)
                        .ThenInclude(s => s.Status)
                    .Include(fs => fs.Ticket)
                        .ThenInclude(t => t.Customer)
                    .Select(fs => new
                    {
                        fs.SeatId,
                        fs.Seat.SeatNumber,
                        fs.IsSat,
                        fs.TicketId,
                        TicketNumber = fs.Ticket != null ? fs.Ticket.TicketNumber : null,
                        CustomerName = fs.Ticket != null && fs.Ticket.Customer != null ? fs.Ticket.Customer.Fullname : null,
                        fs.Seat.ClassId,
                        ClassName = fs.Seat.Class.ClassName,
                        ClassPrice = fs.Seat.Class.Price,
                        SeatStatusId = fs.Seat.StatusId,
                        SeatStatusName = fs.Seat.Status.StatusName
                    })
                    .OrderBy(fs => fs.SeatNumber)
                    .ToListAsync();

                // Get flight services
                var flightServices = await _context.Services
                    .Where(s => s.Flights.Any(f => f.FlightId == id))
                    .Include(s => s.Status)
                    .Select(s => new
                    {
                        s.ServiceId,
                        s.ServiceName,
                        s.Detail,
                        s.StatusId,
                        StatusName = s.Status != null ? s.Status.StatusName : null
                    })
                    .ToListAsync();

                // Calculate seat statistics
                var seatStats = flightSeats
                    .GroupBy(fs => fs.ClassName)
                    .Select(g => new
                    {
                        ClassName = g.Key,
                        TotalSeats = g.Count(),
                        OccupiedSeats = g.Count(s => s.IsSat),
                        AvailableSeats = g.Count(s => !s.IsSat),
                        Revenue = g.Where(s => s.IsSat).Sum(s => s.ClassPrice)
                    })
                    .ToList();

                var totalRevenue = seatStats.Sum(s => s.Revenue);
                var totalSeats = flightSeats.Count;
                var occupiedSeats = flightSeats.Count(fs => fs.IsSat);
                var occupancyRate = totalSeats > 0 ? (double)occupiedSeats / totalSeats * 100 : 0;

                var result = new
                {
                    // Flight basic info
                    FlightId = flight.FlightId,
                    FlightCode = flight.FlightCode,
                    Tax = flight.Tax,
                    DepartureTime = flight.DepartureTime,
                    ArrivalTime = flight.ArrivalTime,
                    
                    // Flight status
                    StatusId = flight.StatusId,
                    StatusName = flight.Status?.StatusName,
                    
                    // Airports
                    DepartureAirport = new
                    {
                        flight.DepartureAirport.AirportId,
                        flight.DepartureAirport.AirportCode,
                        flight.DepartureAirport.AirportName,
                        flight.DepartureAirport.City
                    },
                    ArrivalAirport = new
                    {
                        flight.ArrivalAirport.AirportId,
                        flight.ArrivalAirport.AirportCode,
                        flight.ArrivalAirport.AirportName,
                        flight.ArrivalAirport.City
                    },
                    
                    // Plane
                    Plane = new
                    {
                        flight.Plane.PlaneId,
                        flight.Plane.PlaneCode,
                        flight.Plane.Model,
                        flight.Plane.Manufacture
                    },
                    
                    // Manager
                    Manager = new
                    {
                        flight.Manager.ManagerId,
                        flight.Manager.Fullname
                    },
                    
                    // Seats
                    Seats = flightSeats,
                    SeatStatistics = seatStats,
                    
                    // Services
                    Services = flightServices,
                    
                    // Summary statistics
                    Statistics = new
                    {
                        TotalSeats = totalSeats,
                        OccupiedSeats = occupiedSeats,
                        AvailableSeats = totalSeats - occupiedSeats,
                        OccupancyRate = Math.Round(occupancyRate, 2),
                        TotalRevenue = totalRevenue,
                        TotalServices = flightServices.Count
                    }
                };

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting flight details for flight {FlightId}", id);
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

        [HttpGet("services")]
        public async Task<IActionResult> GetAvailableServices()
        {
            try
            {
                var managerId = await GetManagerIdAsync();
                var services = await _context.Services
                    .Where(s => s.ManagerId == managerId && s.StatusId == 1) // Only active services for this manager
                    .Select(s => new
                    {
                        s.ServiceId,
                        s.ServiceName,
                        s.Detail
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = services });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available services");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("{flightId}/seats")]
        public async Task<IActionResult> GetFlightSeats(int flightId)
        {
            try
            {
                var managerId = await GetManagerIdAsync();
                
                // Check if manager can access this flight
                var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == flightId);
                if (flight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }
                
                if (flight.ManagerId != managerId)
                {
                    return Forbid();
                }

                var flightSeats = await _context.FlightSeats
                    .Where(fs => fs.FlightId == flightId)
                    .Include(fs => fs.Seat)
                        .ThenInclude(s => s.Class)
                    .Include(fs => fs.Seat)
                        .ThenInclude(s => s.Status)
                    .Include(fs => fs.Ticket)
                    .Select(fs => new
                    {
                        fs.FlightId,
                        fs.SeatId,
                        fs.Seat.SeatNumber,
                        fs.IsSat,
                        fs.TicketId,
                        TicketCode = fs.Ticket != null ? fs.Ticket.TicketNumber : null,
                        fs.Seat.ClassId,
                        ClassName = fs.Seat.Class.ClassName,
                        ClassPrice = fs.Seat.Class.Price,
                        SeatStatusId = fs.Seat.StatusId,
                        SeatStatusName = fs.Seat.Status.StatusName
                    })
                    .OrderBy(fs => fs.SeatNumber)
                    .ToListAsync();

                return Ok(new { success = true, data = flightSeats });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting flight seats for flight {FlightId}", flightId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("{flightId}/services")]
        public async Task<IActionResult> GetFlightServices(int flightId)
        {
            try
            {
                var managerId = await GetManagerIdAsync();
                
                // Check if manager can access this flight
                var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == flightId);
                if (flight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }
                
                if (flight.ManagerId != managerId)
                {
                    return Forbid();
                }

                var flightServices = await _context.Services
                    .Where(s => s.Flights.Any(f => f.FlightId == flightId))
                    .Include(s => s.Manager)
                    .Include(s => s.Status)
                    .Select(s => new
                    {
                        s.ServiceId,
                        s.ServiceName,
                        s.Detail,
                        s.ManagerId,
                        ManagerName = s.Manager.Fullname,
                        s.StatusId,
                        StatusName = s.Status != null ? s.Status.StatusName : null
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = flightServices });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting flight services for flight {FlightId}", flightId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("{flightId}/services")]
        public async Task<IActionResult> AddServiceToFlight(int flightId, [FromBody] AddServiceToFlightRequestDTO request)
        {
            try
            {
                var managerId = await GetManagerIdAsync();
                
                // Check if manager can access this flight
                var flight = await _context.Flights
                    .Include(f => f.Services)
                    .FirstOrDefaultAsync(f => f.FlightId == flightId);
                    
                if (flight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }
                
                if (flight.ManagerId != managerId)
                {
                    return Forbid();
                }

                // Check if service exists and is managed by the same manager
                var service = await _context.Services
                    .FirstOrDefaultAsync(s => s.ServiceId == request.ServiceId && 
                                            s.ManagerId == managerId && 
                                            s.StatusId == 1);
                                            
                if (service == null)
                {
                    return BadRequest(new { success = false, message = "Service not found or not accessible" });
                }

                // Check if service is already added to flight
                if (flight.Services.Any(s => s.ServiceId == request.ServiceId))
                {
                    return BadRequest(new { success = false, message = "Service is already added to this flight" });
                }

                // Add service to flight
                flight.Services.Add(service);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Service added to flight successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding service to flight {FlightId}", flightId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpDelete("{flightId}/services/{serviceId}")]
        public async Task<IActionResult> RemoveServiceFromFlight(int flightId, int serviceId)
        {
            try
            {
                var managerId = await GetManagerIdAsync();
                
                // Check if manager can access this flight
                var flight = await _context.Flights
                    .Include(f => f.Services)
                    .FirstOrDefaultAsync(f => f.FlightId == flightId);
                    
                if (flight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }
                
                if (flight.ManagerId != managerId)
                {
                    return Forbid();
                }

                // Find and remove service from flight
                var serviceToRemove = flight.Services.FirstOrDefault(s => s.ServiceId == serviceId);
                if (serviceToRemove == null)
                {
                    return NotFound(new { success = false, message = "Service not found in this flight" });
                }

                flight.Services.Remove(serviceToRemove);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Service removed from flight successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing service from flight {FlightId}", flightId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPut("{flightId}/seats/{seatId}")]
        public async Task<IActionResult> UpdateFlightSeat(int flightId, int seatId, [FromBody] FlightSeatUpdateRequestDTO request)
        {
            try
            {
                var managerId = await GetManagerIdAsync();
                
                // Check if manager can access this flight
                var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == flightId);
                if (flight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }
                
                if (flight.ManagerId != managerId)
                {
                    return Forbid();
                }

                // Find the flight seat
                var flightSeat = await _context.FlightSeats
                    .FirstOrDefaultAsync(fs => fs.FlightId == flightId && fs.SeatId == request.SeatId);
                    
                if (flightSeat == null)
                {
                    return NotFound(new { success = false, message = "Flight seat not found" });
                }

                // Update flight seat
                flightSeat.IsSat = request.IsSat;
                flightSeat.TicketId = request.TicketId;

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Flight seat updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating flight seat {SeatId} for flight {FlightId}", seatId, flightId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("{flightId}/seats/regenerate")]
        public async Task<IActionResult> RegenerateFlightSeats(int flightId)
        {
            try
            {
                var managerId = await GetManagerIdAsync();
                
                // Check if manager can access this flight
                var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == flightId);
                if (flight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }
                
                if (flight.ManagerId != managerId)
                {
                    return Forbid();
                }

                // Store original status to check if it changes
                var originalStatusId = flight.StatusId;

                // Call service method to regenerate flight seats
                await _flightManageService.RegenerateFlightSeats(flightId, flight.PlaneId);

                // Reload flight to check if status changed
                await _context.Entry(flight).ReloadAsync();
                var statusChanged = originalStatusId != flight.StatusId;

                var result = new
                {
                    success = true,
                    message = "Flight seats regenerated successfully",
                    data = new
                    {
                        flightId = flightId,
                        flightStatusChanged = statusChanged,
                        newFlightStatusId = flight.StatusId,
                        newFlightStatusName = flight.StatusId == 1 ? "Active" : "Inactive"
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error regenerating flight seats for flight {FlightId}", flightId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("{flightId}/seats")]
        public async Task<IActionResult> AddFlightSeat(int flightId, [FromBody] AddFlightSeatRequestDTO request)
        {
            try
            {
                var managerId = await GetManagerIdAsync();
                
                // Check if manager can access this flight
                var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == flightId);
                if (flight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }
                
                if (flight.ManagerId != managerId)
                {
                    return Forbid();
                }

                // Check if seat exists and belongs to the same plane
                var seat = await _context.Seats
                    .Include(s => s.Class)
                    .Include(s => s.Status)
                    .FirstOrDefaultAsync(s => s.SeatId == request.SeatId && s.PlaneId == flight.PlaneId);

                if (seat == null)
                {
                    return BadRequest(new { success = false, message = "Seat not found or does not belong to this flight's plane" });
                }

                // Check if FlightSeat already exists
                var existingFlightSeat = await _context.FlightSeats
                    .FirstOrDefaultAsync(fs => fs.FlightId == flightId && fs.SeatId == request.SeatId);

                if (existingFlightSeat != null)
                {
                    return BadRequest(new { success = false, message = "Flight seat already exists for this seat" });
                }

                // Create new FlightSeat
                var flightSeat = new FlightSeat
                {
                    FlightId = flightId,
                    SeatId = request.SeatId,
                    IsSat = request.IsSat ?? false,
                    TicketId = request.TicketId
                };

                _context.FlightSeats.Add(flightSeat);
                await _context.SaveChangesAsync();

                // Auto-activate flight status when flight seat is added
                var statusChanged = false;
                if (flight.StatusId == 2) // If currently Inactive
                {
                    flight.StatusId = 1; // Set to Active
                    await _context.SaveChangesAsync();
                    statusChanged = true;
                    _logger.LogInformation($"Flight {flightId} status automatically changed to Active (1) because a flight seat was added");
                }

                // Return the created flight seat with seat details and flight status info
                var result = new
                {
                    flightSeat.FlightId,
                    flightSeat.SeatId,
                    seat.SeatNumber,
                    flightSeat.IsSat,
                    flightSeat.TicketId,
                    seat.ClassId,
                    ClassName = seat.Class.ClassName,
                    ClassPrice = seat.Class.Price,
                    SeatStatusId = seat.StatusId,
                    SeatStatusName = seat.Status.StatusName,
                    // Include flight status information for realtime update
                    FlightStatusChanged = statusChanged,
                    NewFlightStatusId = flight.StatusId,
                    NewFlightStatusName = flight.StatusId == 1 ? "Active" : "Inactive"
                };

                return Ok(new { success = true, data = result, message = "Flight seat added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding flight seat for flight {FlightId}", flightId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpDelete("{flightId}/seats/{seatId}")]
        public async Task<IActionResult> RemoveFlightSeat(int flightId, int seatId)
        {
            try
            {
                var managerId = await GetManagerIdAsync();
                
                // Check if manager can access this flight
                var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == flightId);
                if (flight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }
                
                if (flight.ManagerId != managerId)
                {
                    return Forbid();
                }

                // Find the flight seat
                var flightSeat = await _context.FlightSeats
                    .FirstOrDefaultAsync(fs => fs.FlightId == flightId && fs.SeatId == seatId);

                if (flightSeat == null)
                {
                    return NotFound(new { success = false, message = "Flight seat not found" });
                }

                // Check if seat is occupied (has a ticket)
                if (flightSeat.TicketId != null)
                {
                    return BadRequest(new { success = false, message = "Cannot remove occupied seat" });
                }

                _context.FlightSeats.Remove(flightSeat);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Flight seat removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing flight seat {SeatId} from flight {FlightId}", seatId, flightId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("{flightId}/available-seats")]
        public async Task<IActionResult> GetAvailableSeatsForFlight(int flightId)
        {
            try
            {
                var managerId = await GetManagerIdAsync();
                
                // Check if manager can access this flight
                var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == flightId);
                if (flight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }
                
                if (flight.ManagerId != managerId)
                {
                    return Forbid();
                }

                // Get seats that are not yet added to this flight
                var usedSeatIds = await _context.FlightSeats
                    .Where(fs => fs.FlightId == flightId)
                    .Select(fs => fs.SeatId)
                    .ToListAsync();

                var availableSeats = await _context.Seats
                    .Where(s => s.PlaneId == flight.PlaneId && 
                               s.StatusId == 1 && 
                               !usedSeatIds.Contains(s.SeatId))
                    .Include(s => s.Class)
                    .Include(s => s.Status)
                    .Select(s => new
                    {
                        s.SeatId,
                        s.SeatNumber,
                        s.PlaneId,
                        s.ClassId,
                        ClassName = s.Class.ClassName,
                        ClassPrice = s.Class.Price,
                        s.StatusId,
                        StatusName = s.Status.StatusName
                    })
                    .OrderBy(s => s.SeatNumber)
                    .ToListAsync();

                return Ok(new { success = true, data = availableSeats });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available seats for flight {FlightId}", flightId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPut("{flightId}/status")]
        public async Task<IActionResult> ChangeFlightStatus(int flightId, [FromBody] ChangeFlightStatusRequestDTO request)
        {
            try
            {
                _logger.LogInformation($"ChangeFlightStatus called for flight {flightId} with statusId {request.StatusId}");
                
                var managerId = await GetManagerIdAsync();
                _logger.LogInformation($"Manager ID: {managerId}");
                
                // Check if flight exists and manager can access it
                var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == flightId);
                if (flight == null)
                {
                    _logger.LogWarning($"Flight {flightId} not found");
                    return NotFound(new { success = false, message = "Flight not found" });
                }
                
                _logger.LogInformation($"Flight found: {flight.FlightCode}, Current status: {flight.StatusId}, Manager: {flight.ManagerId}");
                
                if (flight.ManagerId != managerId)
                {
                    _logger.LogWarning($"Manager {managerId} attempted to access flight {flightId} managed by {flight.ManagerId}");
                    return Forbid();
                }

                // Validate status ID
                if (request.StatusId != 1 && request.StatusId != 2)
                {
                    _logger.LogWarning($"Invalid status ID: {request.StatusId}");
                    return BadRequest(new { success = false, message = "Invalid status ID. Must be 1 (Active) or 2 (Inactive)" });
                }

                var oldStatusId = flight.StatusId;
                var oldStatusName = oldStatusId == 1 ? "Active" : "Inactive";
                var newStatusName = request.StatusId == 1 ? "Active" : "Inactive";

                // Check if status is actually changing
                if (oldStatusId == request.StatusId)
                {
                    _logger.LogInformation($"Flight {flightId} already has status {request.StatusId} ({newStatusName})");
                    return Ok(new { success = true, message = $"Flight is already {newStatusName}" });
                }

                // Update flight status
                flight.StatusId = request.StatusId;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Flight {flightId} status changed from {oldStatusName} ({oldStatusId}) to {newStatusName} ({request.StatusId}) by Manager {managerId}");

                var result = new
                {
                    flightId = flightId,
                    oldStatusId = oldStatusId,
                    oldStatusName = oldStatusName,
                    newStatusId = request.StatusId,
                    newStatusName = newStatusName,
                    changedBy = managerId
                };

                return Ok(new { success = true, data = result, message = $"Flight status changed to {newStatusName} successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing flight status for flight {FlightId}", flightId);
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }
    }
}