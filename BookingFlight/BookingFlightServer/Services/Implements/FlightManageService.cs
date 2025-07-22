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
        private readonly IGeminiAIService _geminiAiService;
        private readonly BookingFlightContext _context;
        private readonly ILogger<FlightManageService> _logger;
        private readonly IEmailService _emailService;

        public FlightManageService(
            IFlightManageRepository flightRepository,
            IGeminiAIService geminiAiService,
            BookingFlightContext context,
            ILogger<FlightManageService> logger,
            IEmailService emailService)
        {
            _flightRepository = flightRepository;
            _geminiAiService = geminiAiService;
            _context = context;
            _logger = logger;
            _emailService = emailService;
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

            // Store old values for notification comparisons
            var oldValues = new
            {
                Tax = existingFlight.Tax,
                DepartureTime = existingFlight.DepartureTime,
                ArrivalTime = existingFlight.ArrivalTime,
                PlaneId = existingFlight.PlaneId,
                DepartureAirportId = existingFlight.DepartureAirportId,
                ArrivalAirportId = existingFlight.ArrivalAirportId
            };

            // Update flight properties
            existingFlight.FlightCode = request.FlightCode;
            existingFlight.Tax = request.Tax;
            existingFlight.DepartureTime = request.DepartureTime;
            existingFlight.ArrivalTime = request.ArrivalTime;
            existingFlight.PlaneId = request.PlaneId;
            existingFlight.DepartureAirportId = request.DepartureAirportId;
            existingFlight.ArrivalAirportId = request.ArrivalAirportId;

            var updatedFlight = await _flightRepository.UpdateFlight(existingFlight);
            
            // If plane changed, update FlightSeats
            if (oldValues.PlaneId != request.PlaneId)
            {
                await UpdateFlightSeatsForNewPlane(updatedFlight.FlightId, request.PlaneId);
            }
            
            // Update services for flight
            await UpdateFlightServices(updatedFlight.FlightId, request.ServiceIds, managerId);

            // Check for any changes and send appropriate notifications
            await DetectAndSendChangeNotifications(updatedFlight.FlightId, oldValues, updatedFlight);
            
            // Load related entities for DTO mapping
            var fullFlight = await _flightRepository.GetFlightById(updatedFlight.FlightId);
            return MapToDTO(fullFlight!);
        }

        /// <summary>
        /// Detect any changes and send appropriate notifications
        /// </summary>
        private async Task DetectAndSendChangeNotifications(int flightId, object oldValues, Flight newFlight)
        {
            try
            {
                // Use reflection to access anonymous object properties safely
                var oldType = oldValues.GetType();
                var oldTax = oldType.GetProperty("Tax")?.GetValue(oldValues);
                var oldDepartureTime = (DateTime?)oldType.GetProperty("DepartureTime")?.GetValue(oldValues);
                var oldArrivalTime = (DateTime?)oldType.GetProperty("ArrivalTime")?.GetValue(oldValues);
                var oldPlaneId = (int?)oldType.GetProperty("PlaneId")?.GetValue(oldValues);
                var oldDepartureAirportId = (int?)oldType.GetProperty("DepartureAirportId")?.GetValue(oldValues);
                var oldArrivalAirportId = (int?)oldType.GetProperty("ArrivalAirportId")?.GetValue(oldValues);
                
                // Check for tax changes
                if (oldTax != null && !oldTax.Equals(newFlight.Tax))
                {
                    await CheckAndSendFlightUpdateNotifications(flightId, oldTax, newFlight.Tax, "tax");
                }
                
                // Check for time changes
                if (oldDepartureTime.HasValue && oldArrivalTime.HasValue && 
                    (oldDepartureTime.Value != newFlight.DepartureTime || oldArrivalTime.Value != newFlight.ArrivalTime))
                {
                    var oldTimeValues = new { DepartureTime = oldDepartureTime.Value, ArrivalTime = oldArrivalTime.Value };
                    var newTimeValues = new { DepartureTime = newFlight.DepartureTime, ArrivalTime = newFlight.ArrivalTime };
                    await CheckAndSendFlightUpdateNotifications(flightId, oldTimeValues, newTimeValues, "time");
                }
                
                // Check for aircraft changes
                if (oldPlaneId.HasValue && oldPlaneId.Value != newFlight.PlaneId)
                {
                    // Get plane information for better display
                    var oldPlane = await _context.Planes.FirstOrDefaultAsync(p => p.PlaneId == oldPlaneId.Value);
                    var newPlane = await _context.Planes.FirstOrDefaultAsync(p => p.PlaneId == newFlight.PlaneId);
                    
                    var oldPlaneInfo = oldPlane != null ? $"{oldPlane.PlaneCode} ({oldPlane.Model})" : $"Máy bay ID: {oldPlaneId.Value}";
                    var newPlaneInfo = newPlane != null ? $"{newPlane.PlaneCode} ({newPlane.Model})" : $"Máy bay ID: {newFlight.PlaneId}";
                    
                    await CheckAndSendFlightUpdateNotifications(flightId, oldPlaneInfo, newPlaneInfo, "aircraft");
                }
                
                // Check for departure airport changes
                if (oldDepartureAirportId.HasValue && oldDepartureAirportId.Value != newFlight.DepartureAirportId)
                {
                    // Get airport information for better display
                    var oldAirport = await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == oldDepartureAirportId.Value);
                    var newAirport = await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == newFlight.DepartureAirportId);
                    
                    var oldAirportInfo = oldAirport?.AirportName ?? $"Sân bay ID: {oldDepartureAirportId.Value}";
                    var newAirportInfo = newAirport?.AirportName ?? $"Sân bay ID: {newFlight.DepartureAirportId}";
                    
                    await CheckAndSendFlightUpdateNotifications(flightId, $"Sân bay khởi hành: {oldAirportInfo}", $"Sân bay khởi hành: {newAirportInfo}", "gate");
                }
                
                // Check for arrival airport changes
                if (oldArrivalAirportId.HasValue && oldArrivalAirportId.Value != newFlight.ArrivalAirportId)
                {
                    // Get airport information for better display
                    var oldAirport = await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == oldArrivalAirportId.Value);
                    var newAirport = await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == newFlight.ArrivalAirportId);
                    
                    var oldAirportInfo = oldAirport?.AirportName ?? $"Sân bay ID: {oldArrivalAirportId.Value}";
                    var newAirportInfo = newAirport?.AirportName ?? $"Sân bay ID: {newFlight.ArrivalAirportId}";
                    
                    await CheckAndSendFlightUpdateNotifications(flightId, $"Sân bay đến: {oldAirportInfo}", $"Sân bay đến: {newAirportInfo}", "terminal");
                }
                
                _logger.LogInformation($"Change detection completed for flight {flightId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error detecting changes for flight {flightId}");
                // Don't throw - email failure shouldn't fail the flight update
            }
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

                    // AI analysis would go here if needed
                    result.AiAnalysis = "Flight conflicts detected";
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

        /// <summary>
        /// Check for occupied seats and send email notifications when flight is updated (any property changes)
        /// </summary>
        private async Task CheckAndSendFlightUpdateNotifications(int flightId, object oldValues = null, object newValues = null, string changeType = "time")
        {
            try
            {
                // Get all occupied seats (IsSat = true) for this flight with ticket information
                var occupiedSeats = await _context.FlightSeats
                    .Where(fs => fs.FlightId == flightId && fs.IsSat && fs.TicketId.HasValue)
                    .Include(fs => fs.Ticket)
                    .Include(fs => fs.Flight)
                        .ThenInclude(f => f.DepartureAirport)
                    .Include(fs => fs.Flight)
                        .ThenInclude(f => f.ArrivalAirport)
                    .ToListAsync();

                if (!occupiedSeats.Any())
                {
                    _logger.LogInformation($"No occupied seats found for flight {flightId}. No notifications sent.");
                    return;
                }

                _logger.LogInformation($"Found {occupiedSeats.Count} occupied seats for flight {flightId}. Sending email notifications for {changeType} changes.");

                // Group by ticket email to avoid duplicate emails
                var ticketGroups = occupiedSeats
                    .Where(fs => fs.Ticket != null && !string.IsNullOrEmpty(fs.Ticket.ContactEmail))
                    .GroupBy(fs => fs.Ticket.ContactEmail)
                    .ToList();

                foreach (var ticketGroup in ticketGroups)
                {
                    var firstTicket = ticketGroup.First().Ticket;
                    var flight = ticketGroup.First().Flight;
                    var tickets = ticketGroup.Select(fs => fs.Ticket).Where(t => t != null).ToList();

                    if (firstTicket != null)
                    {
                        await SendFlightUpdateEmailWithChangeType(firstTicket, flight, tickets!, oldValues, newValues, changeType);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending flight update notifications for flight {flightId}");
                // Don't throw - email failure shouldn't fail the flight update
            }
        }

        /// <summary>
        /// Check for occupied seats and send email notifications when flight is updated (backward compatibility)
        /// </summary>
        private async Task CheckAndSendFlightUpdateNotifications(int flightId, DateTime oldDepartureTime, DateTime oldArrivalTime)
        {
            // Create old/new values objects for time changes
            var oldValues = new { DepartureTime = oldDepartureTime, ArrivalTime = oldArrivalTime };
            var newValues = new { DepartureTime = DateTime.MinValue, ArrivalTime = DateTime.MinValue }; // Will be filled from current flight data
            
            await CheckAndSendFlightUpdateNotifications(flightId, oldValues, newValues, "time");
        }

        /// <summary>
        /// Send email notification to ticket holder about flight update
        /// </summary>
        private async Task SendFlightUpdateEmail(Ticket ticket, Flight flight, List<Ticket> tickets, DateTime oldDepartureTime, DateTime oldArrivalTime)
        {
            try
            {
                var ticketNumbers = string.Join(", ", tickets.Select(t => t.TicketNumber));
                var ticketCount = tickets.Count;
                
                // Get customer name from ticket - use ContactFullName if available, otherwise use FullName
                var customerName = !string.IsNullOrEmpty(ticket.ContactFullName) ? ticket.ContactFullName : ticket.FullName;
                var customerEmail = ticket.ContactEmail;

                if (string.IsNullOrEmpty(customerEmail))
                {
                    _logger.LogWarning($"No email found for ticket {ticket.TicketNumber}. Cannot send notification.");
                    return;
                }

                // Generate AI-powered reason for flight change
                var aiReason = await _geminiAiService.GenerateFlightUpdateReasonAsync(
                    flight.FlightCode, 
                    oldDepartureTime, 
                    flight.DepartureTime,
                    oldArrivalTime, 
                    flight.ArrivalTime);

                var subject = $"Thông báo thay đổi chuyến bay {flight.FlightCode}";
                
                var body = $@"
                <!DOCTYPE html>
                <html lang=""vi"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Thông Báo Chuyến Bay</title>
                </head>
                <body style=""margin: 0; padding: 20px; font-family: Arial, sans-serif; background-color: #f0f2f5; line-height: 1.6;"">
                    
                    <!-- Main Container -->
                    <table cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""max-width: 650px; margin: 0 auto; background-color: white; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1);"">
                        
                        <!-- Header -->
                        <tr>
                            <td style=""background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%); color: white; padding: 30px 20px; text-align: center;"">
                                <div style=""font-size: 40px; margin-bottom: 10px;"">✈️</div>
                                <h1 style=""margin: 0; font-size: 28px; font-weight: bold;"">THÔNG BÁO CHUYẾN BAY</h1>
                                <p style=""margin: 10px 0 0 0; font-size: 14px; opacity: 0.9;"">BookingFlight - Dịch vụ hàng không tin cậy</p>
                            </td>
                        </tr>
                        
                        <!-- Main Content -->
                        <tr>
                            <td style=""padding: 30px;"">
                                
                                <!-- Greeting -->
                                <div style=""border-left: 4px solid #4facfe; padding-left: 20px; margin-bottom: 25px; background-color: #f8f9fa; padding: 15px 20px; border-radius: 5px;"">
                                    <p style=""font-size: 16px; color: #333; margin: 0; font-weight: 500;"">
                                        Kính gửi quý khách <strong style=""color: #4facfe;"">{customerName}</strong>,
                                    </p>
                                </div>
                                
                                <p style=""font-size: 16px; color: #555; margin-bottom: 25px;"">
                                    Chúng tôi xin thông báo về việc điều chỉnh lịch trình chuyến bay <strong style=""color: #e74c3c; font-size: 18px;"">{flight.FlightCode}</strong> 
                                    mà quý khách đã đặt vé. Chúng tôi rất xin lỗi vì sự bất tiện này.
                                </p>
                                
                                <!-- AI Reason Box - Simplified -->
                                <div style=""background-color: #fff3cd; border: 2px solid #ffc107; border-left: 6px solid #ff8f00; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                                    <h3 style=""color: #e65100; margin: 0 0 15px 0; font-size: 18px; font-weight: bold;"">
                                        🔍 Lý do thay đổi lịch trình
                                    </h3>
                                    <div style=""background-color: white; padding: 15px; border-radius: 5px; border: 1px solid #ffc107;"">
                                        <p style=""color: #5d4037; margin: 0; font-size: 15px; font-style: italic; font-weight: 500;"">
                                            {aiReason}
                                        </p>
                                    </div>
                                </div>
                                
                                <!-- Ticket Info -->
                                <div style=""background-color: #e8f5e8; border: 2px solid #28a745; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                                    <h3 style=""color: #155724; margin: 0 0 15px 0; font-size: 18px; font-weight: bold;"">
                                        ✓ Thông tin vé của quý khách
                                    </h3>
                                    <p style=""margin: 5px 0; color: #2d5016; font-size: 14px;""><strong>Số vé:</strong> 
                                        <span style=""background-color: #007bff; color: white; padding: 5px 10px; border-radius: 15px; font-weight: bold;"">{ticketNumbers}</span>
                                    </p>
                                    <p style=""margin: 5px 0; color: #2d5016; font-size: 14px;""><strong>Số lượng vé:</strong> 
                                        <span style=""background-color: #dc3545; color: white; padding: 5px 15px; border-radius: 50%; font-weight: bold; font-size: 16px;"">{ticketCount}</span>
                                    </p>
                                </div>
                                
                                <!-- Schedule Changes -->
                                <div style=""border: 2px solid #dc3545; border-radius: 8px; overflow: hidden; margin: 25px 0;"">
                                    <div style=""background-color: #dc3545; color: white; padding: 15px 20px;"">
                                        <h3 style=""margin: 0; font-size: 18px; font-weight: bold;"">📅 Chi tiết thay đổi lịch trình</h3>
                                    </div>
                                    <table style=""width: 100%; border-collapse: collapse; background-color: white;"">
                                        <thead>
                                            <tr style=""background-color: #f8f9fa;"">
                                                <th style=""border: 1px solid #dee2e6; padding: 12px; text-align: left; font-weight: bold;"">Thông tin</th>
                                                <th style=""border: 1px solid #dee2e6; padding: 12px; text-align: center; font-weight: bold;"">Lịch cũ</th>
                                                <th style=""border: 1px solid #dee2e6; padding: 12px; text-align: center; font-weight: bold;"">Lịch mới</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td style=""border: 1px solid #dee2e6; padding: 12px; font-weight: bold;"">🛫 Giờ khởi hành</td>
                                                <td style=""border: 1px solid #dee2e6; padding: 12px; text-align: center; background-color: #fff3cd; color: #856404; font-weight: bold;"">{oldDepartureTime:dd/MM/yyyy HH:mm}</td>
                                                <td style=""border: 1px solid #dee2e6; padding: 12px; text-align: center; background-color: #d1ecf1; color: #0c5460; font-weight: bold; font-size: 16px;"">{flight.DepartureTime:dd/MM/yyyy HH:mm}</td>
                                            </tr>
                                            <tr>
                                                <td style=""border: 1px solid #dee2e6; padding: 12px; font-weight: bold;"">🛬 Giờ đến</td>
                                                <td style=""border: 1px solid #dee2e6; padding: 12px; text-align: center; background-color: #fff3cd; color: #856404; font-weight: bold;"">{oldArrivalTime:dd/MM/yyyy HH:mm}</td>
                                                <td style=""border: 1px solid #dee2e6; padding: 12px; text-align: center; background-color: #d1ecf1; color: #0c5460; font-weight: bold; font-size: 16px;"">{flight.ArrivalTime:dd/MM/yyyy HH:mm}</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                                
                                <!-- Flight Info -->
                                <div style=""background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%); color: white; padding: 25px; border-radius: 10px; margin: 25px 0;"">
                                    <h3 style=""color: white; margin: 0 0 20px 0; font-size: 20px; font-weight: bold;"">✈️ Thông tin chuyến bay</h3>
                                    <div style=""display: block;"">
                                        <div style=""margin-bottom: 15px;"">
                                            <p style=""margin: 5px 0; opacity: 0.9; font-size: 14px;""><strong>Mã chuyến bay:</strong></p>
                                            <p style=""margin: 0; font-size: 22px; font-weight: bold;"">{flight.FlightCode}</p>
                                        </div>
                                        <div>
                                            <p style=""margin: 5px 0; opacity: 0.9; font-size: 14px;""><strong>Tuyến bay:</strong></p>
                                            <p style=""margin: 0; font-size: 16px; font-weight: bold;"">{flight.DepartureAirport?.AirportName} → {flight.ArrivalAirport?.AirportName}</p>
                                            <p style=""margin: 5px 0 0 0; font-size: 14px; opacity: 0.8;"">({flight.DepartureAirport?.AirportCode} → {flight.ArrivalAirport?.AirportCode})</p>
                                        </div>
                                    </div>
                                </div>
                                
                                <!-- Important Notice -->
                                <div style=""background-color: #fff3cd; border: 2px solid #ffc107; border-left: 6px solid #ff8f00; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                                    <h3 style=""color: #e65100; margin: 0 0 15px 0; font-size: 16px; font-weight: bold;"">
                                        ⚠️ Lưu ý quan trọng
                                    </h3>
                                    <ul style=""color: #856404; margin: 10px 0 0 20px; line-height: 1.8;"">
                                        <li style=""margin-bottom: 8px;""><strong>✓</strong> Vui lòng có mặt tại sân bay ít nhất 2 giờ trước giờ khởi hành mới</li>
                                        <li style=""margin-bottom: 8px;""><strong>✓</strong> Kiểm tra lại thông tin chuyến bay và sắp xếp lịch trình phù hợp</li>
                                        <li style=""margin-bottom: 8px;""><strong>✓</strong> Vé của quý khách vẫn có hiệu lực với lịch trình mới</li>
                                        <li style=""margin-bottom: 0;""><strong>✓</strong> Liên hệ hotline nếu cần hỗ trợ thêm</li>
                                    </ul>
                                </div>
                                
                                <!-- Apology -->
                                <div style=""text-align: center; background-color: #f8f9fa; padding: 25px; border-radius: 8px; margin: 25px 0;"">
                                    <div style=""font-size: 30px; margin-bottom: 15px;"">🙏</div>
                                    <p style=""font-size: 16px; color: #495057; margin: 0; line-height: 1.6;"">
                                        Chúng tôi chân thành xin lỗi vì sự bất tiện này và cảm ơn sự thông cảm của quý khách. 
                                        <br><strong style=""color: #007bff;"">Mọi thắc mắc xin vui lòng liên hệ với chúng tôi.</strong>
                                    </p>
                                </div>
                            </td>
                        </tr>
                        
                        <!-- Footer -->
                        <tr>
                            <td style=""background-color: #343a40; color: white; padding: 25px 20px; text-align: center;"">
                                <div style=""border-bottom: 1px solid #495057; padding-bottom: 15px; margin-bottom: 15px;"">
                                    <h3 style=""margin: 0; color: #f8f9fa; font-size: 18px; font-weight: bold;"">📞 Thông tin liên hệ</h3>
                                </div>
                                <div style=""margin-bottom: 15px;"">
                                    <p style=""margin: 5px 0; color: #adb5bd; font-size: 14px;""><strong style=""color: #f8f9fa;"">📧 Email:</strong></p>
                                    <p style=""margin: 0; color: #17a2b8; font-size: 15px; font-weight: bold;"">hainthe172574@fpt.edu.vn</p>
                                </div>
                                <div style=""margin-bottom: 15px;"">
                                    <p style=""margin: 5px 0; color: #adb5bd; font-size: 14px;""><strong style=""color: #f8f9fa;"">☎️ Hotline:</strong></p>
                                    <p style=""margin: 0; color: #28a745; font-size: 18px; font-weight: bold;"">1900-FLIGHT</p>
                                </div>
                                <div style=""padding-top: 15px; border-top: 1px solid #495057;"">
                                    <p style=""margin: 0; color: #6c757d; font-size: 12px;"">
                                        © 2024 BookingFlight. Đây là email tự động, vui lòng không trả lời email này.
                                        <br>Cảm ơn quý khách đã tin tưởng và sử dụng dịch vụ của chúng tôi. ✨
                                    </p>
                                </div>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";

                await _emailService.SendFlightUpdateNotificationAsync(customerEmail, subject, body);
                
                _logger.LogInformation($"Enhanced flight update notification sent to {customerName} at {customerEmail} for flight {flight.FlightCode}, tickets: {ticketNumbers}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending flight update email for ticket {ticket.TicketNumber} for flight {flight.FlightCode}");
            }
        }

        public async Task SendFlightChangeNotificationAsync(
            string flightCode,
            string changeType,
            object oldValue,
            object newValue,
            Dictionary<string, object>? additionalContext = null)
        {
            try
            {
                // Get flight first to get FlightId
                var flight = await _context.Flights
                    .FirstOrDefaultAsync(f => f.FlightCode == flightCode);

                if (flight == null)
                {
                    _logger.LogWarning($"Flight {flightCode} not found");
                    return;
                }

                // Get all tickets for this flight
                var tickets = await _context.Tickets
                    .Include(t => t.Status)
                    .Where(t => t.FlightId == flight.FlightId && t.Status.StatusName != "Cancelled")
                    .ToListAsync();

                if (!tickets.Any())
                {
                    _logger.LogWarning($"No active tickets found for flight {flightCode}");
                    return;
                }

                // Group tickets by email to avoid sending duplicate emails
                var ticketGroups = tickets
                    .Where(t => !string.IsNullOrEmpty(t.ContactEmail))
                    .GroupBy(t => t.ContactEmail)
                    .ToList();

                foreach (var group in ticketGroups)
                {
                    var email = group.Key;
                    var ticketsForEmail = group.ToList();
                    var representativeTicket = ticketsForEmail.First();

                    await SendFlightChangeEmail(representativeTicket, ticketsForEmail, flightCode, changeType, oldValue, newValue, additionalContext);
                }

                _logger.LogInformation($"Flight change notifications sent for {ticketGroups.Count} unique emails for flight {flightCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending flight change notifications for flight {flightCode}");
            }
        }

        private async Task SendFlightChangeEmail(
            Ticket ticket,
            List<Ticket> tickets,
            string flightCode,
            string changeType,
            object oldValue,
            object newValue,
            Dictionary<string, object>? additionalContext)
        {
            try
            {
                var customerName = !string.IsNullOrEmpty(ticket.ContactFullName) ? ticket.ContactFullName : ticket.FullName;
                var customerEmail = ticket.ContactEmail;

                if (string.IsNullOrEmpty(customerEmail))
                {
                    _logger.LogWarning($"No email found for ticket {ticket.TicketNumber}. Cannot send notification.");
                    return;
                }

                // Generate AI-powered reason for flight change
                var aiReason = await _geminiAiService.GenerateFlightChangeReasonAsync(
                    flightCode, changeType, oldValue, newValue, additionalContext);

                var ticketNumbers = string.Join(", ", tickets.Select(t => t.TicketNumber));
                var changeTypeVi = GetChangeTypeInVietnamese(changeType);
                var subject = $"Thông báo thay đổi {changeTypeVi} - Chuyến bay {flightCode}";
                
                var body = BuildFlightChangeEmailBody(customerName, flightCode, changeTypeVi, oldValue, newValue, aiReason, ticketNumbers);

                await _emailService.SendFlightUpdateNotificationAsync(customerEmail, subject, body);
                
                _logger.LogInformation($"Flight change notification ({changeType}) sent to {customerName} at {customerEmail} for flight {flightCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending flight change email for ticket {ticket.TicketNumber} for flight {flightCode}");
            }
        }

        private string GetChangeTypeInVietnamese(string changeType)
        {
            return changeType.ToLower() switch
            {
                "tax" => "thuế và phí",
                "aircraft" => "loại máy bay",
                "gate" => "cổng khởi hành",
                "terminal" => "nhà ga",
                "price" => "giá vé",
                "departuretime" => "giờ khởi hành",
                "arrivaltime" => "giờ đến",
                "route" => "tuyến đường bay",
                "seat" => "chỗ ngồi",
                _ => "thông tin chuyến bay"
            };
        }

        private string BuildFlightChangeEmailBody(
            string customerName,
            string flightCode,
            string changeTypeVi,
            object oldValue,
            object newValue,
            string aiReason,
            string ticketNumbers)
        {
            return $@"
                <!DOCTYPE html>
                <html lang=""vi"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Thông Báo Thay Đổi Chuyến Bay</title>
                </head>
                <body style=""margin: 0; padding: 20px; font-family: Arial, sans-serif; background-color: #f0f2f5; line-height: 1.6;"">
                    
                    <!-- Main Container -->
                    <table style=""max-width: 600px; margin: 0 auto; background-color: white; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.15);"">
                        
                        <!-- Header -->
                        <tr>
                            <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px 20px; text-align: center;"">
                                <div style=""font-size: 32px; margin-bottom: 10px;"">✈️</div>
                                <h1 style=""margin: 0; font-size: 24px; font-weight: bold;"">THÔNG BÁO THAY ĐỔI CHUYẾN BAY</h1>
                                <p style=""margin: 8px 0 0 0; color: rgba(255,255,255,0.9); font-size: 16px;"">Chuyến bay: <strong>{flightCode}</strong></p>
                            </td>
                        </tr>
                        
                        <!-- Content -->
                        <tr>
                            <td style=""padding: 30px 20px;"">
                                <!-- Greeting -->
                                <div style=""background-color: #e3f2fd; padding: 20px; border-radius: 8px; margin-bottom: 25px; border-left: 4px solid #2196f3;"">
                                    <h2 style=""color: #1565c0; margin: 0 0 10px 0; font-size: 20px;"">
                                        Kính chào {customerName}! 👋
                                    </h2>
                                    <p style=""margin: 0; color: #1976d2; font-size: 15px;"">
                                        Chúng tôi xin thông báo về việc thay đổi <strong>{changeTypeVi}</strong> mà quý khách đã đặt vé.
                                        <br><strong>Mã vé:</strong> <span style=""color: #d32f2f; font-weight: bold;"">{ticketNumbers}</span>
                                    </p>
                                </div>

                                <!-- Change Details -->
                                <div style=""background-color: #fff3e0; padding: 20px; border-radius: 8px; margin-bottom: 25px; border-left: 4px solid #ff9800;"">
                                    <h3 style=""color: #e65100; margin: 0 0 15px 0; font-size: 18px;"">
                                        📋 Chi tiết thay đổi
                                    </h3>
                                    <table style=""width: 100%; border-collapse: collapse;"">
                                        <tr>
                                            <td style=""padding: 8px 0; color: #e65100; font-weight: bold; width: 120px;"">Loại thay đổi:</td>
                                            <td style=""padding: 8px 0; color: #bf360c;"">{changeTypeVi}</td>
                                        </tr>
                                        <tr>
                                            <td style=""padding: 8px 0; color: #e65100; font-weight: bold;"">Thông tin cũ:</td>
                                            <td style=""padding: 8px 0; color: #bf360c; text-decoration: line-through;"">{oldValue}</td>
                                        </tr>
                                        <tr>
                                            <td style=""padding: 8px 0; color: #e65100; font-weight: bold;"">Thông tin mới:</td>
                                            <td style=""padding: 8px 0; color: #2e7d32; font-weight: bold;"">{newValue}</td>
                                        </tr>
                                    </table>
                                </div>

                                <!-- AI Reason -->
                                <div style=""background-color: #f3e5f5; padding: 20px; border-radius: 8px; margin-bottom: 25px; border-left: 4px solid #9c27b0;"">
                                    <h3 style=""color: #6a1b9a; margin: 0 0 15px 0; font-size: 18px;"">
                                        🔍 Lý do thay đổi
                                    </h3>
                                    <p style=""color: #4a148c; margin: 0; font-style: italic; line-height: 1.6; font-size: 15px;"">
                                        {aiReason}
                                    </p>
                                </div>

                                <!-- Important Notes -->
                                <div style=""background-color: #fff8e1; padding: 20px; border-radius: 8px; margin-bottom: 25px; border-left: 4px solid #ffc107;"">
                                    <h3 style=""color: #f57c00; margin: 0 0 15px 0; font-size: 18px;"">
                                        ⚠️ Lưu ý quan trọng
                                    </h3>
                                    <ul style=""color: #ef6c00; margin: 10px 0 0 20px; line-height: 1.8;"">
                                        <li style=""margin-bottom: 8px;""><strong>✓</strong> Vé của quý khách vẫn có hiệu lực với thông tin mới</li>
                                        <li style=""margin-bottom: 8px;""><strong>✓</strong> Vui lòng kiểm tra lại thông tin chuyến bay</li>
                                        <li style=""margin-bottom: 8px;""><strong>✓</strong> Không cần thực hiện thêm thủ tục nào</li>
                                        <li style=""margin-bottom: 0;""><strong>✓</strong> Liên hệ hotline nếu cần hỗ trợ thêm</li>
                                    </ul>
                                </div>
                                
                                <!-- Apology -->
                                <div style=""text-align: center; background-color: #f8f9fa; padding: 25px; border-radius: 8px; margin: 25px 0;"">
                                    <div style=""font-size: 30px; margin-bottom: 15px;"">🙏</div>
                                    <p style=""font-size: 16px; color: #495057; margin: 0; line-height: 1.6;"">
                                        Chúng tôi chân thành xin lỗi vì sự bất tiện này và cảm ơn sự thông cảm của quý khách. 
                                        <br><strong style=""color: #007bff;"">Mọi thắc mắc xin vui lòng liên hệ với chúng tôi.</strong>
                                    </p>
                                </div>
                            </td>
                        </tr>
                        
                        <!-- Footer -->
                        <tr>
                            <td style=""background-color: #343a40; color: white; padding: 25px 20px; text-align: center;"">
                                <div style=""border-bottom: 1px solid #495057; padding-bottom: 15px; margin-bottom: 15px;"">
                                    <h3 style=""margin: 0; color: #f8f9fa; font-size: 18px; font-weight: bold;"">📞 Thông tin liên hệ</h3>
                                </div>
                                <div style=""margin-bottom: 15px;"">
                                    <p style=""margin: 5px 0; color: #adb5bd; font-size: 14px;""><strong style=""color: #f8f9fa;"">📧 Email:</strong></p>
                                    <p style=""margin: 0; color: #17a2b8; font-size: 15px; font-weight: bold;"">hainthe172574@fpt.edu.vn</p>
                                </div>
                                <div style=""margin-bottom: 15px;"">
                                    <p style=""margin: 5px 0; color: #adb5bd; font-size: 14px;""><strong style=""color: #f8f9fa;"">☎️ Hotline:</strong></p>
                                    <p style=""margin: 0; color: #28a745; font-size: 18px; font-weight: bold;"">1900-FLIGHT</p>
                                </div>
                                <div style=""padding-top: 15px; border-top: 1px solid #495057;"">
                                    <p style=""margin: 0; color: #6c757d; font-size: 12px;"">
                                        © 2024 BookingFlight. Đây là email tự động, vui lòng không trả lời email này.
                                        <br>Cảm ơn quý khách đã tin tưởng và sử dụng dịch vụ của chúng tôi. ✨
                                    </p>
                                </div>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";
        }

        /// <summary>
        /// Send flight update email with any type of changes (tax, aircraft, gate, time, etc.)
        /// </summary>
        private async Task SendFlightUpdateEmailWithChangeType(Ticket ticket, Flight flight, List<Ticket> tickets, object oldValues, object newValues, string changeType)
        {
            try
            {
                var ticketNumbers = string.Join(", ", tickets.Select(t => t.TicketNumber));
                var ticketCount = tickets.Count;
                
                // Get customer name from ticket - use ContactFullName if available, otherwise use FullName
                var customerName = !string.IsNullOrEmpty(ticket.ContactFullName) ? ticket.ContactFullName : ticket.FullName;
                var customerEmail = ticket.ContactEmail;

                if (string.IsNullOrEmpty(customerEmail))
                {
                    _logger.LogWarning($"No email found for ticket {ticket.TicketNumber}. Cannot send notification.");
                    return;
                }

                // Generate AI-powered reason for flight change based on change type
                string aiReason;
                string changeDescription = "";
                string oldValueStr = "";
                string newValueStr = "";

                // Prepare change-specific information
                switch (changeType.ToLower())
                {
                    case "time":
                        var oldTimeValues = oldValues as dynamic;
                        changeDescription = "thay đổi thời gian";
                        oldValueStr = $"Khởi hành: {((DateTime?)oldTimeValues?.DepartureTime)?.ToString("dd/MM/yyyy HH:mm") ?? "N/A"}, Đến: {((DateTime?)oldTimeValues?.ArrivalTime)?.ToString("dd/MM/yyyy HH:mm") ?? "N/A"}";
                        newValueStr = $"Khởi hành: {flight.DepartureTime:dd/MM/yyyy HH:mm}, Đến: {flight.ArrivalTime:dd/MM/yyyy HH:mm}";
                        aiReason = await _geminiAiService.GenerateFlightChangeReasonAsync(flight.FlightCode, changeType, oldValues, newValues);
                        break;
                    case "tax":
                        changeDescription = "thay đổi phí thuế";
                        oldValueStr = $"Thuế cũ: {oldValues}";
                        newValueStr = $"Thuế mới: {newValues}";
                        aiReason = await _geminiAiService.GenerateFlightChangeReasonAsync(flight.FlightCode, changeType, oldValues, newValues);
                        break;
                    case "aircraft":
                        changeDescription = "thay đổi máy bay";
                        oldValueStr = $"Máy bay cũ: {oldValues}";
                        newValueStr = $"Máy bay mới: {newValues}";
                        aiReason = await _geminiAiService.GenerateFlightChangeReasonAsync(flight.FlightCode, changeType, oldValues, newValues);
                        break;
                    case "gate":
                        changeDescription = "thay đổi cổng";
                        oldValueStr = $"Cổng cũ: {oldValues}";
                        newValueStr = $"Cổng mới: {newValues}";
                        aiReason = await _geminiAiService.GenerateFlightChangeReasonAsync(flight.FlightCode, changeType, oldValues, newValues);
                        break;
                    case "terminal":
                        changeDescription = "thay đổi nhà ga";
                        oldValueStr = $"Nhà ga cũ: {oldValues}";
                        newValueStr = $"Nhà ga mới: {newValues}";
                        aiReason = await _geminiAiService.GenerateFlightChangeReasonAsync(flight.FlightCode, changeType, oldValues, newValues);
                        break;
                    case "price":
                        changeDescription = "thay đổi giá vé";
                        oldValueStr = $"Giá cũ: {oldValues}";
                        newValueStr = $"Giá mới: {newValues}";
                        aiReason = await _geminiAiService.GenerateFlightChangeReasonAsync(flight.FlightCode, changeType, oldValues, newValues);
                        break;
                    default:
                        changeDescription = "thay đổi thông tin";
                        oldValueStr = $"Thông tin cũ: {oldValues}";
                        newValueStr = $"Thông tin mới: {newValues}";
                        aiReason = await _geminiAiService.GenerateFlightChangeReasonAsync(flight.FlightCode, changeType, oldValues, newValues);
                        break;
                }

                var subject = $"Thông báo {changeDescription} chuyến bay {flight.FlightCode}";
                
                var body = $@"
                <!DOCTYPE html>
                <html lang=""vi"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Thông Báo Chuyến Bay</title>
                </head>
                <body style=""margin: 0; padding: 20px; font-family: Arial, sans-serif; background-color: #f0f2f5; line-height: 1.6;"">
                    
                    <!-- Main Container -->
                    <table cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""max-width: 650px; margin: 0 auto; background-color: white; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1);"">
                        
                        <!-- Header -->
                        <tr>
                            <td style=""background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%); color: white; padding: 30px 20px; text-align: center;"">
                                <div style=""font-size: 40px; margin-bottom: 10px;"">✈️</div>
                                <h1 style=""margin: 0; font-size: 28px; font-weight: bold;"">THÔNG BÁO CHUYẾN BAY</h1>
                                <p style=""margin: 10px 0 0 0; font-size: 14px; opacity: 0.9;"">BookingFlight - Dịch vụ hàng không tin cậy</p>
                            </td>
                        </tr>
                        
                        <!-- Main Content -->
                        <tr>
                            <td style=""padding: 30px;"">
                                
                                <!-- Greeting -->
                                <div style=""border-left: 4px solid #4facfe; padding-left: 20px; margin-bottom: 25px; background-color: #f8f9fa; padding: 15px 20px; border-radius: 5px;"">
                                    <p style=""font-size: 16px; color: #333; margin: 0; font-weight: 500;"">
                                        Kính gửi quý khách <strong style=""color: #4facfe;"">{customerName}</strong>,
                                    </p>
                                </div>
                                
                                <p style=""font-size: 16px; color: #555; margin-bottom: 25px;"">
                                    Chúng tôi xin thông báo về việc <strong style=""color: #e74c3c; font-size: 18px;"">{changeDescription}</strong> 
                                    của chuyến bay <strong style=""color: #e74c3c; font-size: 18px;"">{flight.FlightCode}</strong> 
                                    mà quý khách đã đặt vé. Chúng tôi rất xin lỗi vì sự bất tiện này.
                                </p>
                                
                                <!-- AI Reason Box - Simplified -->
                                <div style=""background-color: #fff3cd; border: 2px solid #ffc107; border-left: 6px solid #ff8f00; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                                    <h3 style=""color: #e65100; margin: 0 0 15px 0; font-size: 18px; font-weight: bold;"">
                                        🔍 Lý do {changeDescription}
                                    </h3>
                                    <div style=""background-color: white; padding: 15px; border-radius: 5px; border: 1px solid #ffc107;"">
                                        <p style=""color: #5d4037; margin: 0; font-size: 15px; font-style: italic; font-weight: 500;"">
                                            {aiReason}
                                        </p>
                                    </div>
                                </div>
                                
                                <!-- Ticket Info -->
                                <div style=""background-color: #e8f5e8; border: 2px solid #28a745; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                                    <h3 style=""color: #155724; margin: 0 0 15px 0; font-size: 18px; font-weight: bold;"">
                                        ✓ Thông tin vé của quý khách
                                    </h3>
                                    <p style=""margin: 5px 0; color: #2d5016; font-size: 14px;""><strong>Số vé:</strong> 
                                        <span style=""background-color: #007bff; color: white; padding: 5px 10px; border-radius: 15px; font-weight: bold;"">{ticketNumbers}</span>
                                    </p>
                                    <p style=""margin: 5px 0; color: #2d5016; font-size: 14px;""><strong>Số lượng vé:</strong> 
                                        <span style=""background-color: #dc3545; color: white; padding: 5px 15px; border-radius: 50%; font-weight: bold; font-size: 16px;"">{ticketCount}</span>
                                    </p>
                                </div>
                                
                                <!-- Change Details -->
                                <div style=""border: 2px solid #dc3545; border-radius: 8px; overflow: hidden; margin: 25px 0;"">
                                    <div style=""background-color: #dc3545; color: white; padding: 15px 20px;"">
                                        <h3 style=""margin: 0; font-size: 18px; font-weight: bold;"">📅 Chi tiết {changeDescription}</h3>
                                    </div>
                                    <table style=""width: 100%; border-collapse: collapse; background-color: white;"">
                                        <thead>
                                            <tr style=""background-color: #f8f9fa;"">
                                                <th style=""border: 1px solid #dee2e6; padding: 12px; text-align: left; font-weight: bold;"">Thông tin</th>
                                                <th style=""border: 1px solid #dee2e6; padding: 12px; text-align: center; font-weight: bold;"">Trước đây</th>
                                                <th style=""border: 1px solid #dee2e6; padding: 12px; text-align: center; font-weight: bold;"">Hiện tại</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td style=""border: 1px solid #dee2e6; padding: 12px; font-weight: bold;"">🔄 {changeDescription.Substring(0, 1).ToUpper() + changeDescription.Substring(1)}</td>
                                                <td style=""border: 1px solid #dee2e6; padding: 12px; text-align: center; background-color: #fff3cd; color: #856404; font-weight: bold;"">{oldValueStr}</td>
                                                <td style=""border: 1px solid #dee2e6; padding: 12px; text-align: center; background-color: #d1ecf1; color: #0c5460; font-weight: bold; font-size: 16px;"">{newValueStr}</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                                
                                <!-- Flight Info -->
                                <div style=""background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%); color: white; padding: 25px; border-radius: 10px; margin: 25px 0;"">
                                    <h3 style=""color: white; margin: 0 0 20px 0; font-size: 20px; font-weight: bold;"">✈️ Thông tin chuyến bay</h3>
                                    <div style=""display: block;"">
                                        <div style=""margin-bottom: 15px;"">
                                            <p style=""margin: 5px 0; opacity: 0.9; font-size: 14px;""><strong>Mã chuyến bay:</strong></p>
                                            <p style=""margin: 0; font-size: 22px; font-weight: bold;"">{flight.FlightCode}</p>
                                        </div>
                                        <div>
                                            <p style=""margin: 5px 0; opacity: 0.9; font-size: 14px;""><strong>Tuyến bay:</strong></p>
                                            <p style=""margin: 0; font-size: 16px; font-weight: bold;"">{flight.DepartureAirport?.AirportName} → {flight.ArrivalAirport?.AirportName}</p>
                                            <p style=""margin: 5px 0 0 0; font-size: 14px; opacity: 0.8;"">({flight.DepartureAirport?.AirportCode} → {flight.ArrivalAirport?.AirportCode})</p>
                                        </div>
                                    </div>
                                </div>
                                
                                <!-- Important Notice -->
                                <div style=""background-color: #fff3cd; border: 2px solid #ffc107; border-left: 6px solid #ff8f00; padding: 20px; border-radius: 8px; margin: 25px 0;"">
                                    <h3 style=""color: #e65100; margin: 0 0 15px 0; font-size: 16px; font-weight: bold;"">
                                        ⚠️ Lưu ý quan trọng
                                    </h3>
                                    <ul style=""color: #856404; margin: 10px 0 0 20px; line-height: 1.8;"">
                                        <li style=""margin-bottom: 8px;""><strong>✓</strong> Vui lòng kiểm tra lại thông tin chuyến bay trước khi khởi hành</li>
                                        <li style=""margin-bottom: 8px;""><strong>✓</strong> Sắp xếp lịch trình phù hợp với thông tin mới</li>
                                        <li style=""margin-bottom: 8px;""><strong>✓</strong> Vé của quý khách vẫn có hiệu lực với thông tin cập nhật</li>
                                        <li style=""margin-bottom: 0;""><strong>✓</strong> Liên hệ hotline nếu cần hỗ trợ thêm</li>
                                    </ul>
                                </div>
                                
                                <!-- Apology -->
                                <div style=""text-align: center; background-color: #f8f9fa; padding: 25px; border-radius: 8px; margin: 25px 0;"">
                                    <div style=""font-size: 30px; margin-bottom: 15px;"">🙏</div>
                                    <p style=""font-size: 16px; color: #495057; margin: 0; line-height: 1.6;"">
                                        Chúng tôi chân thành xin lỗi vì sự bất tiện này và cảm ơn sự thông cảm của quý khách. 
                                        <br><strong style=""color: #007bff;"">Mọi thắc mắc xin vui lòng liên hệ với chúng tôi.</strong>
                                    </p>
                                </div>
                            </td>
                        </tr>
                        
                        <!-- Footer -->
                        <tr>
                            <td style=""background-color: #343a40; color: white; padding: 25px 20px; text-align: center;"">
                                <div style=""border-bottom: 1px solid #495057; padding-bottom: 15px; margin-bottom: 15px;"">
                                    <h3 style=""margin: 0; color: #f8f9fa; font-size: 18px; font-weight: bold;"">📞 Thông tin liên hệ</h3>
                                </div>
                                <div style=""margin-bottom: 15px;"">
                                    <p style=""margin: 5px 0; color: #adb5bd; font-size: 14px;""><strong style=""color: #f8f9fa;"">📧 Email:</strong></p>
                                    <p style=""margin: 0; color: #17a2b8; font-size: 15px; font-weight: bold;"">hainthe172574@fpt.edu.vn</p>
                                </div>
                                <div style=""margin-bottom: 15px;"">
                                    <p style=""margin: 5px 0; color: #adb5bd; font-size: 14px;""><strong style=""color: #f8f9fa;"">☎️ Hotline:</strong></p>
                                    <p style=""margin: 0; color: #28a745; font-size: 18px; font-weight: bold;"">1900-FLIGHT</p>
                                </div>
                                <div style=""padding-top: 15px; border-top: 1px solid #495057;"">
                                    <p style=""margin: 0; color: #6c757d; font-size: 12px;"">
                                        © 2024 BookingFlight. Đây là email tự động, vui lòng không trả lời email này.
                                        <br>Cảm ơn quý khách đã tin tưởng và sử dụng dịch vụ của chúng tôi. ✨
                                    </p>
                                </div>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>";

                await _emailService.SendFlightUpdateNotificationAsync(customerEmail, subject, body);
                
                _logger.LogInformation($"Enhanced flight update notification sent to {customerName} at {customerEmail} for flight {flight.FlightCode}, tickets: {ticketNumbers}, change type: {changeType}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending flight update email for ticket {ticket.TicketNumber} for flight {flight.FlightCode}, change type: {changeType}");
            }
        }
    }
}
