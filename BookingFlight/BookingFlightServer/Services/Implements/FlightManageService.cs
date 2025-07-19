using BookingFlightServer.DTO.Manager;
using BookingFlightServer.DTO.Shared;
using BookingFlightServer.Entities;
using BookingFlightServer.Repositories;
using BookingFlightServer.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BookingFlightServer.Services.Implements
{
    public class FlightManageService : IFlightManageService
    {
        private readonly IFlightManageRepository _flightRepository;
        private readonly IGeminiAIService _geminiAIService;
        private readonly BookingFlightContext _context;
        private readonly ILogger<FlightManageService> _logger;

        public FlightManageService(
            IFlightManageRepository flightRepository,
            IGeminiAIService geminiAIService,
            BookingFlightContext context,
            ILogger<FlightManageService> logger)
        {
            _flightRepository = flightRepository;
            _geminiAIService = geminiAIService;
            _context = context;
            _logger = logger;
        }

        public async Task<List<FlightManageDTO>> GetFlightsByFilter(FlightListRequestDTO request)
        {
            var flights = await _flightRepository.GetFlightsByFilter(request);
            return flights.Select(MapToDTO).ToList();
        }

        public async Task<int> GetTotalFlightsCount(FlightListRequestDTO request)
        {
            return await _flightRepository.GetTotalFlightsCount(request);
        }

        public async Task<FlightManageDTO?> GetFlightById(int flightId)
        {
            var flight = await _flightRepository.GetFlightById(flightId);
            return flight != null ? MapToDTO(flight) : null;
        }

        public async Task<FlightManageDTO> CreateFlight(FlightCreateRequestDTO request, int managerId)
        {
            // Check if flight code exists
            if (await _flightRepository.IsFlightCodeExists(request.FlightCode))
            {
                throw new InvalidOperationException("Flight code already exists");
            }

            // Validate times
            if (request.DepartureTime >= request.ArrivalTime)
            {
                throw new InvalidOperationException("Departure time must be before arrival time");
            }

            // Validate departure time must be at least 2 days from now
            var minimumDepartureTime = DateTime.Now.AddDays(2);
            if (request.DepartureTime < minimumDepartureTime)
            {
                throw new InvalidOperationException($"Departure time must be at least 2 days from now (minimum: {minimumDepartureTime:yyyy-MM-dd HH:mm})");
            }

            // Check for conflicts
            var conflictCheck = new FlightConflictCheckRequestDTO
            {
                DepartureTime = request.DepartureTime,
                ArrivalTime = request.ArrivalTime,
                PlaneId = request.PlaneId,
                DepartureAirportId = request.DepartureAirportId,
                ArrivalAirportId = request.ArrivalAirportId
            };

            var conflictResult = await CheckFlightConflicts(conflictCheck);
            if (conflictResult.HasConflict)
            {
                throw new InvalidOperationException($"Flight scheduling conflicts detected: {string.Join(", ", conflictResult.Conflicts.Select(c => c.Description))}");
            }

            var flight = new Flight
            {
                FlightCode = request.FlightCode,
                Tax = request.Tax,
                DepartureTime = request.DepartureTime,
                ArrivalTime = request.ArrivalTime,
                PlaneId = request.PlaneId,
                ManagerId = managerId, // Use logged-in manager
                DepartureAirportId = request.DepartureAirportId,
                ArrivalAirportId = request.ArrivalAirportId,
                StatusId = 2 // Default to Inactive status
            };

            var createdFlight = await _flightRepository.CreateFlight(flight);
            _logger.LogInformation($"✅ Created flight {createdFlight.FlightId} with code {createdFlight.FlightCode}");
            
            // Note: FlightSeats can be added manually later through the seat management interface
            _logger.LogInformation($"📝 Flight created successfully. Seats can be added manually via seat management.");
            
            try
            {
                // Add services to flight if any specified
                if (request.ServiceIds != null && request.ServiceIds.Any())
                {
                    _logger.LogInformation($"🛎️ Adding {request.ServiceIds.Count} services to Flight {createdFlight.FlightId}");
                    await AddServicesToFlight(createdFlight.FlightId, request.ServiceIds, managerId);
                }
                else
                {
                    _logger.LogInformation($"ℹ️ No services specified for Flight {createdFlight.FlightId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Failed to add services to Flight {createdFlight.FlightId}");
                // Don't fail the entire operation, but log the error
            }
            
            // Load related entities for DTO mapping
            var fullFlight = await _flightRepository.GetFlightById(createdFlight.FlightId);
            return MapToDTO(fullFlight!);
        }

        public async Task<FlightManageDTO> UpdateFlight(FlightUpdateRequestDTO request, int managerId)
        {
            var existingFlight = await _flightRepository.GetFlightById(request.FlightId);
            if (existingFlight == null)
            {
                throw new InvalidOperationException("Flight not found");
            }

            // Check if manager can edit this flight
            if (existingFlight.ManagerId != managerId)
            {
                throw new UnauthorizedAccessException("You can only edit flights you manage");
            }

            // Check if departure time is in the past - only allow updates for future flights
            if (existingFlight.DepartureTime < DateTime.Now)
            {
                throw new InvalidOperationException("Cannot update flights that have already departed");
            }

            // Check if flight code exists (excluding current flight)
            if (await _flightRepository.IsFlightCodeExists(request.FlightCode, request.FlightId))
            {
                throw new InvalidOperationException("Flight code already exists");
            }

            // Validate times
            if (request.DepartureTime >= request.ArrivalTime)
            {
                throw new InvalidOperationException("Departure time must be before arrival time");
            }

            // Check for conflicts
            var conflictCheck = new FlightConflictCheckRequestDTO
            {
                FlightId = request.FlightId,
                DepartureTime = request.DepartureTime,
                ArrivalTime = request.ArrivalTime,
                PlaneId = request.PlaneId,
                DepartureAirportId = request.DepartureAirportId,
                ArrivalAirportId = request.ArrivalAirportId
            };

            var conflictResult = await CheckFlightConflicts(conflictCheck);
            if (conflictResult.HasConflict)
            {
                throw new InvalidOperationException($"Flight scheduling conflicts detected: {string.Join(", ", conflictResult.Conflicts.Select(c => c.Description))}");
            }

            // Update flight properties
            var oldPlaneId = existingFlight.PlaneId;
            existingFlight.FlightCode = request.FlightCode;
            existingFlight.Tax = request.Tax;
            existingFlight.DepartureTime = request.DepartureTime;
            existingFlight.ArrivalTime = request.ArrivalTime;
            existingFlight.PlaneId = request.PlaneId;
            existingFlight.DepartureAirportId = request.DepartureAirportId;
            existingFlight.ArrivalAirportId = request.ArrivalAirportId;
            // Don't update StatusId - it's managed automatically by the system
            // existingFlight.StatusId = request.StatusId;

            var updatedFlight = await _flightRepository.UpdateFlight(existingFlight);
            
            // If plane changed, update FlightSeats
            if (oldPlaneId != request.PlaneId)
            {
                await UpdateFlightSeatsForNewPlane(updatedFlight.FlightId, request.PlaneId);
            }
            
            // Update services for flight
            await UpdateFlightServices(updatedFlight.FlightId, request.ServiceIds, managerId);
            
            // Load related entities for DTO mapping
            var fullFlight = await _flightRepository.GetFlightById(updatedFlight.FlightId);
            return MapToDTO(fullFlight!);
        }

        public async Task<bool> DeleteFlight(int flightId)
        {
            return await _flightRepository.DeleteFlight(flightId);
        }

        public async Task<FlightConflictResultDTO> CheckFlightConflicts(FlightConflictCheckRequestDTO request)
        {
            var conflicts = await _flightRepository.CheckFlightConflicts(request);
            var result = new FlightConflictResultDTO
            {
                HasConflict = conflicts.Any()
            };

            if (conflicts.Any())
            {
                result.Conflicts = conflicts.Select(f => new FlightConflictDetailDTO
                {
                    FlightId = f.FlightId,
                    FlightCode = f.FlightCode,
                    DepartureTime = f.DepartureTime,
                    ArrivalTime = f.ArrivalTime,
                    ConflictType = DetermineConflictType(f, request),
                    Description = GenerateConflictDescription(f, request)
                }).ToList();

                // Get AI analysis
                try
                {
                    var conflictData = JsonSerializer.Serialize(new
                    {
                        ProposedFlight = request,
                        ConflictingFlights = result.Conflicts,
                        Timestamp = DateTime.Now
                    });

                    result.AiAnalysis = await _geminiAIService.AnalyzeFlightConflicts(conflictData);
                    result.Recommendations = ExtractRecommendations(result.AiAnalysis);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting AI analysis for flight conflicts");
                    result.AiAnalysis = "AI analysis unavailable";
                }
            }

            return result;
        }

        public async Task<List<FlightManageDTO>> GetFlightsByManagerId(int managerId)
        {
            var flights = await _flightRepository.GetFlightsByManagerId(managerId);
            return flights.Select(MapToDTO).ToList();
        }

        public async Task<List<StatusDTO>> GetFlightStatuses()
        {
            return await _context.Statuses
                .Select(s => new StatusDTO
                {
                    StatusId = s.StatusId,
                    StatusName = s.StatusName
                })
                .ToListAsync();
        }

        private FlightManageDTO MapToDTO(Flight flight)
        {
            // Calculate seat statistics
            var totalSeats = _context.FlightSeats.Count(fs => fs.FlightId == flight.FlightId);
            var bookedSeats = _context.FlightSeats.Count(fs => fs.FlightId == flight.FlightId && fs.IsSat);

            // Get services for this flight with detailed information
            var flightServices = _context.Services
                .Where(s => s.Flights.Any(f => f.FlightId == flight.FlightId))
                .Include(s => s.Manager)
                .Include(s => s.Status)
                .Select(s => new FlightServiceDTO
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName,
                    Detail = s.Detail,
                    ManagerId = s.ManagerId,
                    ManagerName = s.Manager.Fullname,
                    StatusId = s.StatusId,
                    StatusName = s.Status != null ? s.Status.StatusName : null
                })
                .ToList();

            // Get flight seats with detailed information
            var flightSeats = _context.FlightSeats
                .Where(fs => fs.FlightId == flight.FlightId)
                .Include(fs => fs.Seat)
                .ThenInclude(s => s.Class)
                .Include(fs => fs.Seat)
                .ThenInclude(s => s.Status)
                .Include(fs => fs.Ticket)
                .Select(fs => new FlightSeatDTO
                {
                    FlightId = fs.FlightId,
                    SeatId = fs.SeatId,
                    SeatNumber = fs.Seat.SeatNumber,
                    IsSat = fs.IsSat,
                    TicketId = fs.TicketId,
                    TicketCode = fs.Ticket != null ? fs.Ticket.TicketNumber : null,
                    ClassId = fs.Seat.ClassId,
                    ClassName = fs.Seat.Class.ClassName,
                    ClassPrice = fs.Seat.Class.Price,
                    SeatStatusId = fs.Seat.StatusId,
                    SeatStatusName = fs.Seat.Status.StatusName
                })
                .OrderBy(fs => fs.SeatNumber)
                .ToList();

            return new FlightManageDTO
            {
                FlightId = flight.FlightId,
                FlightCode = flight.FlightCode,
                Tax = flight.Tax,
                StatusId = flight.StatusId,
                StatusName = flight.Status?.StatusName,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                PlaneId = flight.PlaneId,
                PlaneName = flight.Plane?.PlaneCode,
                ManagerId = flight.ManagerId,
                ManagerName = flight.Manager?.Fullname,

                DepartureAirportId = flight.DepartureAirportId,
                DepartureAirportName = flight.DepartureAirport?.AirportName,
                DepartureAirportCode = flight.DepartureAirport?.AirportCode,
                ArrivalAirportId = flight.ArrivalAirportId,
                ArrivalAirportName = flight.ArrivalAirport?.AirportName,
                ArrivalAirportCode = flight.ArrivalAirport?.AirportCode,
                BasePrice = GetBasePrice(flight.DepartureAirportId, flight.ArrivalAirportId),
                TotalSeats = totalSeats,
                BookedSeats = bookedSeats,
                AvailableSeats = totalSeats - bookedSeats,
                Services = flightServices,
                FlightSeats = flightSeats
            };
        }

        private decimal GetBasePrice(int departureAirportId, int arrivalAirportId)
        {
            try
            {
                var airportPrice = _context.AirportPrices
                    .FirstOrDefault(ap => ap.AirportFromId == departureAirportId && ap.AirportToId == arrivalAirportId);
                return airportPrice?.BasePrice ?? 1000000; // Default price if not found
            }
            catch
            {
                return 1000000; // Default price on error
            }
        }

        private string DetermineConflictType(Flight conflictingFlight, FlightConflictCheckRequestDTO request)
        {
            // Now we only check plane conflicts
            if (conflictingFlight.PlaneId == request.PlaneId)
                return "PlaneConflict";
            return "TimeConflict";
        }

        private string GenerateConflictDescription(Flight conflictingFlight, FlightConflictCheckRequestDTO request)
        {
            // Since we only check plane conflicts now, this will always be plane conflict
            return $"Aircraft {conflictingFlight.Plane?.PlaneCode} is already scheduled for flight {conflictingFlight.FlightCode} during the requested time period";
        }

        private List<string> ExtractRecommendations(string aiAnalysis)
        {
            var recommendations = new List<string>();
            
            if (string.IsNullOrWhiteSpace(aiAnalysis))
                return recommendations;

            // Simple extraction logic - look for numbered recommendations
            var lines = aiAnalysis.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("1.") || line.Trim().StartsWith("2.") || 
                    line.Trim().StartsWith("3.") || line.Trim().StartsWith("4.") ||
                    line.Trim().StartsWith("-") || line.Trim().StartsWith("•"))
                {
                    recommendations.Add(line.Trim());
                }
            }

            if (!recommendations.Any())
            {
                recommendations.Add("Select a different aircraft for this time slot");
                recommendations.Add("Adjust departure/arrival times to avoid aircraft conflicts");
                recommendations.Add("Schedule flight when the aircraft is available");
            }

            return recommendations;
        }

        /// <summary>
        /// Creates FlightSeat records for all seats in the specified plane
        /// </summary>
        private async Task CreateFlightSeatsForFlight(int flightId, int planeId)
        {
            try
            {
                _logger.LogInformation($"Starting to create FlightSeats for Flight {flightId}, Plane {planeId}");
                
                // Ensure the plane has seats before creating FlightSeats
                await EnsurePlaneHasSeats(planeId);
                
                // Get all seats for the plane
                var planeSeats = await _context.Seats
                    .Where(s => s.PlaneId == planeId && s.StatusId == 1) // Only active seats
                    .Include(s => s.Class)
                    .Include(s => s.Status)
                    .ToListAsync();

                _logger.LogInformation($"Found {planeSeats.Count} active seats for Plane {planeId}");

                if (!planeSeats.Any())
                {
                    _logger.LogWarning($"No active seats found for Plane {planeId}. Checking all seats...");
                    
                    // Check if there are any seats at all for this plane
                    var allSeats = await _context.Seats
                        .Where(s => s.PlaneId == planeId)
                        .Include(s => s.Status)
                        .ToListAsync();
                    
                    _logger.LogInformation($"Plane {planeId} has {allSeats.Count} total seats");
                    foreach (var seat in allSeats.Take(5)) // Log first 5 seats for debugging
                    {
                        _logger.LogInformation($"Seat {seat.SeatId}: {seat.SeatNumber}, StatusId: {seat.StatusId}, Status: {seat.Status?.StatusName}");
                    }
                    
                    if (allSeats.Count == 0)
                    {
                        _logger.LogError($"No seats exist for Plane {planeId}. Cannot create FlightSeats.");
                        return;
                    }
                    
                    // If no active seats but seats exist, log this issue
                    _logger.LogWarning($"Plane {planeId} has {allSeats.Count} seats but none are active (StatusId = 1)");
                    return;
                }

                // Check if FlightSeats already exist for this flight
                var existingFlightSeats = await _context.FlightSeats
                    .Where(fs => fs.FlightId == flightId)
                    .ToListAsync();

                if (existingFlightSeats.Any())
                {
                    _logger.LogInformation($"FlightSeats already exist for Flight {flightId} ({existingFlightSeats.Count} seats). Skipping creation.");
                    return;
                }

                // Create FlightSeat records
                var flightSeats = planeSeats.Select(seat => new FlightSeat
                {
                    FlightId = flightId,
                    SeatId = seat.SeatId,
                    IsSat = false, // Initially not occupied
                    TicketId = null
                }).ToList();

                _logger.LogInformation($"About to create {flightSeats.Count} FlightSeat records for Flight {flightId}");

                if (flightSeats.Any())
                {
                    _context.FlightSeats.AddRange(flightSeats);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Successfully created {flightSeats.Count} FlightSeat records for Flight {flightId}");
                    
                    // Auto-activate flight status when flight seats are created
                    var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == flightId);
                    if (flight != null && flight.StatusId == 2) // If currently Inactive
                    {
                        flight.StatusId = 1; // Set to Active
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"Flight {flightId} status automatically changed to Active (1) because flight seats were created");
                    }
                    
                    // Verify the creation
                    var createdCount = await _context.FlightSeats.CountAsync(fs => fs.FlightId == flightId);
                    _logger.LogInformation($"Verification: {createdCount} FlightSeats now exist for Flight {flightId}");
                }
                else
                {
                    _logger.LogWarning($"No FlightSeats to create for Flight {flightId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating FlightSeats for Flight {flightId}, Plane {planeId}");
                throw;
            }
        }

        /// <summary>
        /// Creates default seats for a plane if none exist
        /// </summary>
        private async Task EnsurePlaneHasSeats(int planeId)
        {
            try
            {
                var existingSeats = await _context.Seats
                    .Where(s => s.PlaneId == planeId)
                    .CountAsync();

                if (existingSeats > 0)
                {
                    _logger.LogInformation($"Plane {planeId} already has {existingSeats} seats");
                    return;
                }

                _logger.LogInformation($"Creating default seats for Plane {planeId}");

                // Get default classes (assuming we have Economy, Business, First class)
                var economyClass = await _context.ClassSeats.FirstOrDefaultAsync(c => c.ClassName.ToLower().Contains("economy"));
                var businessClass = await _context.ClassSeats.FirstOrDefaultAsync(c => c.ClassName.ToLower().Contains("business"));
                var firstClass = await _context.ClassSeats.FirstOrDefaultAsync(c => c.ClassName.ToLower().Contains("first"));

                if (economyClass == null)
                {
                    _logger.LogWarning("No Economy class found. Cannot create default seats.");
                    return;
                }

                var defaultSeats = new List<Seat>();

                // Create First Class seats (rows 1-2, seats A-D) - 8 seats
                if (firstClass != null)
                {
                    for (int row = 1; row <= 2; row++)
                    {
                        foreach (char seatLetter in new[] { 'A', 'B', 'C', 'D' })
                        {
                            defaultSeats.Add(new Seat
                            {
                                SeatNumber = $"{row}{seatLetter}",
                                PlaneId = planeId,
                                ClassId = firstClass.ClassId,
                                StatusId = 1 // Active
                            });
                        }
                    }
                }

                // Create Business Class seats (rows 3-5, seats A-F) - 18 seats
                if (businessClass != null)
                {
                    for (int row = 3; row <= 5; row++)
                    {
                        foreach (char seatLetter in new[] { 'A', 'B', 'C', 'D', 'E', 'F' })
                        {
                            defaultSeats.Add(new Seat
                            {
                                SeatNumber = $"{row}{seatLetter}",
                                PlaneId = planeId,
                                ClassId = businessClass.ClassId,
                                StatusId = 1 // Active
                            });
                        }
                    }
                }

                // Create Economy Class seats (rows 6-35, seats A-F) - 180 seats
                for (int row = 6; row <= 35; row++)
                {
                    foreach (char seatLetter in new[] { 'A', 'B', 'C', 'D', 'E', 'F' })
                    {
                        defaultSeats.Add(new Seat
                        {
                            SeatNumber = $"{row}{seatLetter}",
                            PlaneId = planeId,
                            ClassId = economyClass.ClassId,
                            StatusId = 1 // Active
                        });
                    }
                }

                if (defaultSeats.Any())
                {
                    _context.Seats.AddRange(defaultSeats);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Created {defaultSeats.Count} default seats for Plane {planeId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating default seats for Plane {planeId}");
                // Don't rethrow - this is a helper method, not critical
            }
        }

        /// <summary>
        /// Updates FlightSeats when plane is changed - removes old seats and creates new ones
        /// Only for seats that are not already booked
        /// </summary>
        private async Task UpdateFlightSeatsForNewPlane(int flightId, int newPlaneId)
        {
            try
            {
                // Get existing FlightSeats that are not booked (IsSat = false and TicketId = null)
                var existingUnbookedSeats = await _context.FlightSeats
                    .Where(fs => fs.FlightId == flightId && !fs.IsSat && fs.TicketId == null)
                    .ToListAsync();

                // Remove unbooked seats
                if (existingUnbookedSeats.Any())
                {
                    _context.FlightSeats.RemoveRange(existingUnbookedSeats);
                    _logger.LogInformation($"Removed {existingUnbookedSeats.Count} unbooked FlightSeat records for Flight {flightId}");
                }

                // Create new FlightSeats for the new plane
                await CreateFlightSeatsForFlight(flightId, newPlaneId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating FlightSeats for Flight {flightId} with new Plane {newPlaneId}");
                throw;
            }
        }

        /// <summary>
        /// Adds services to a flight (only services managed by the same manager)
        /// </summary>
        private async Task AddServicesToFlight(int flightId, List<int> serviceIds, int managerId)
        {
            if (serviceIds == null || !serviceIds.Any())
                return;

            try
            {
                // Get the flight with services
                var flight = await _context.Flights
                    .Include(f => f.Services)
                    .FirstOrDefaultAsync(f => f.FlightId == flightId);
                    
                if (flight == null)
                {
                    _logger.LogWarning($"Flight {flightId} not found when adding services");
                    return;
                }

                _logger.LogInformation($"Found Flight {flightId}, attempting to add services: {string.Join(", ", serviceIds)}");

                // Get valid services (only those managed by the same manager and active)
                var validServices = await _context.Services
                    .Where(s => serviceIds.Contains(s.ServiceId) && 
                               s.ManagerId == managerId && 
                               s.StatusId == 1) // Active services only
                    .ToListAsync();

                _logger.LogInformation($"Found {validServices.Count} valid services out of {serviceIds.Count} requested");

                if (validServices.Any())
                {
                    var addedCount = 0;
                    // Add services to flight
                    foreach (var service in validServices)
                    {
                        if (!flight.Services.Any(s => s.ServiceId == service.ServiceId))
                        {
                            flight.Services.Add(service);
                            addedCount++;
                            _logger.LogInformation($"Added service {service.ServiceId} ({service.ServiceName}) to flight {flightId}");
                        }
                        else
                        {
                            _logger.LogInformation($"Service {service.ServiceId} already exists in flight {flightId}");
                        }
                    }

                    if (addedCount > 0)
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"Successfully added {addedCount} services to Flight {flightId}");
                    }
                }
                else
                {
                    _logger.LogWarning($"No valid services found for Flight {flightId}. Requested: {string.Join(", ", serviceIds)}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding services to Flight {flightId}");
                throw;
            }
        }

        /// <summary>
        /// Updates services for a flight - removes old services and adds new ones
        /// </summary>
        private async Task UpdateFlightServices(int flightId, List<int> serviceIds, int managerId)
        {
            try
            {
                // Get the flight with its current services
                var flight = await _context.Flights
                    .Include(f => f.Services)
                    .FirstOrDefaultAsync(f => f.FlightId == flightId);

                if (flight == null)
                {
                    _logger.LogWarning($"Flight {flightId} not found when updating services");
                    return;
                }

                // Remove all current services
                flight.Services.Clear();

                // Add new services if any specified
                if (serviceIds != null && serviceIds.Any())
                {
                    var validServices = await _context.Services
                        .Where(s => serviceIds.Contains(s.ServiceId) && 
                                   s.ManagerId == managerId && 
                                   s.StatusId == 1) // Active services only
                        .ToListAsync();

                    foreach (var service in validServices)
                    {
                        flight.Services.Add(service);
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Updated services for Flight {flightId}. New service count: {flight.Services.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating services for Flight {flightId}");
                throw;
            }
        }

        /// <summary>
        /// Regenerates FlightSeats for a flight - useful when plane configuration changes
        /// </summary>
        public async Task RegenerateFlightSeats(int flightId, int planeId)
        {
            try
            {
                // Remove all existing FlightSeats that are not booked
                var existingUnbookedSeats = await _context.FlightSeats
                    .Where(fs => fs.FlightId == flightId && !fs.IsSat && fs.TicketId == null)
                    .ToListAsync();

                if (existingUnbookedSeats.Any())
                {
                    _context.FlightSeats.RemoveRange(existingUnbookedSeats);
                    _logger.LogInformation($"Removed {existingUnbookedSeats.Count} unbooked FlightSeat records for Flight {flightId}");
                }

                // Create new FlightSeats for the plane
                await CreateFlightSeatsForFlight(flightId, planeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error regenerating FlightSeats for Flight {flightId}");
                throw;
            }
        }
    }
}
