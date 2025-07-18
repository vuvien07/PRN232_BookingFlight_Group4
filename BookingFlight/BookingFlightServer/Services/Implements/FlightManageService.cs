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
                StatusId = request.StatusId
            };

            var createdFlight = await _flightRepository.CreateFlight(flight);
            
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
            existingFlight.FlightCode = request.FlightCode;
            existingFlight.Tax = request.Tax;
            existingFlight.DepartureTime = request.DepartureTime;
            existingFlight.ArrivalTime = request.ArrivalTime;
            existingFlight.PlaneId = request.PlaneId;
            existingFlight.DepartureAirportId = request.DepartureAirportId;
            existingFlight.ArrivalAirportId = request.ArrivalAirportId;
            existingFlight.StatusId = request.StatusId;

            var updatedFlight = await _flightRepository.UpdateFlight(existingFlight);
            
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
                AvailableSeats = totalSeats - bookedSeats
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
    }
}
