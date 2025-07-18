using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using BookingFlightServer.DTO.Manager;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
    public class FlightManageRepository : IFlightManageRepository
    {
        private readonly BookingFlightContext _context;

        public FlightManageRepository(BookingFlightContext context)
        {
            _context = context;
        }

        public async Task<List<Flight>> GetFlightsByFilter(FlightListRequestDTO request)
        {
            var query = _context.Flights
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .Include(f => f.Plane)
                .Include(f => f.Manager)
                .Include(f => f.Status)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(f => 
                    f.FlightCode.Contains(request.SearchTerm) ||
                    f.DepartureAirport.AirportName.Contains(request.SearchTerm) ||
                    f.ArrivalAirport.AirportName.Contains(request.SearchTerm) ||
                    f.Plane.PlaneCode.Contains(request.SearchTerm));
            }

            if (request.StatusId.HasValue)
            {
                query = query.Where(f => f.StatusId == request.StatusId.Value);
            }

            if (request.ManagerId.HasValue)
            {
                query = query.Where(f => f.ManagerId == request.ManagerId.Value);
            }

            if (request.DepartureFrom.HasValue)
            {
                query = query.Where(f => f.DepartureTime >= request.DepartureFrom.Value);
            }

            if (request.DepartureTo.HasValue)
            {
                query = query.Where(f => f.DepartureTime <= request.DepartureTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.DepartureAirport))
            {
                query = query.Where(f => f.DepartureAirport.AirportCode.Contains(request.DepartureAirport) ||
                                        f.DepartureAirport.AirportName.Contains(request.DepartureAirport));
            }

            if (!string.IsNullOrWhiteSpace(request.ArrivalAirport))
            {
                query = query.Where(f => f.ArrivalAirport.AirportCode.Contains(request.ArrivalAirport) ||
                                        f.ArrivalAirport.AirportName.Contains(request.ArrivalAirport));
            }

            return await query
                .OrderByDescending(f => f.DepartureTime)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalFlightsCount(FlightListRequestDTO request)
        {
            var query = _context.Flights
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .Include(f => f.Plane)
                .AsQueryable();

            // Apply same filters as GetFlightsByFilter
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(f => 
                    f.FlightCode.Contains(request.SearchTerm) ||
                    f.DepartureAirport.AirportName.Contains(request.SearchTerm) ||
                    f.ArrivalAirport.AirportName.Contains(request.SearchTerm) ||
                    f.Plane.PlaneCode.Contains(request.SearchTerm));
            }

            if (request.StatusId.HasValue)
            {
                query = query.Where(f => f.StatusId == request.StatusId.Value);
            }

            if (request.ManagerId.HasValue)
            {
                query = query.Where(f => f.ManagerId == request.ManagerId.Value);
            }

            if (request.DepartureFrom.HasValue)
            {
                query = query.Where(f => f.DepartureTime >= request.DepartureFrom.Value);
            }

            if (request.DepartureTo.HasValue)
            {
                query = query.Where(f => f.DepartureTime <= request.DepartureTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.DepartureAirport))
            {
                query = query.Where(f => f.DepartureAirport.AirportCode.Contains(request.DepartureAirport) ||
                                        f.DepartureAirport.AirportName.Contains(request.DepartureAirport));
            }

            if (!string.IsNullOrWhiteSpace(request.ArrivalAirport))
            {
                query = query.Where(f => f.ArrivalAirport.AirportCode.Contains(request.ArrivalAirport) ||
                                        f.ArrivalAirport.AirportName.Contains(request.ArrivalAirport));
            }

            return await query.CountAsync();
        }

        public async Task<Flight?> GetFlightById(int flightId)
        {
            return await _context.Flights
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .Include(f => f.Plane)
                .Include(f => f.Manager)
                .Include(f => f.Status)
                .FirstOrDefaultAsync(f => f.FlightId == flightId);
        }

        public async Task<Flight> CreateFlight(Flight flight)
        {
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();
            return flight;
        }

        public async Task<Flight> UpdateFlight(Flight flight)
        {
            _context.Flights.Update(flight);
            await _context.SaveChangesAsync();
            return flight;
        }

        public async Task<bool> DeleteFlight(int flightId)
        {
            var flight = await _context.Flights.FindAsync(flightId);
            if (flight == null) return false;

            // Check if flight has associated tickets
            var hasTickets = await _context.Tickets.AnyAsync(t => t.FlightId == flightId);
            if (hasTickets)
            {
                // Soft delete - change status to inactive
                flight.StatusId = 2; // Assuming 2 is inactive status
                _context.Flights.Update(flight);
            }
            else
            {
                // Hard delete if no tickets
                _context.Flights.Remove(flight);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsFlightCodeExists(string flightCode, int? excludeFlightId = null)
        {
            var query = _context.Flights.Where(f => f.FlightCode == flightCode);
            
            if (excludeFlightId.HasValue)
            {
                query = query.Where(f => f.FlightId != excludeFlightId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<List<Flight>> CheckFlightConflicts(FlightConflictCheckRequestDTO request)
        {
            var query = _context.Flights
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .Include(f => f.Plane)
                .AsQueryable();

            // Exclude current flight if updating
            if (request.FlightId.HasValue)
            {
                query = query.Where(f => f.FlightId != request.FlightId.Value);
            }

            var requestDeparture = request.DepartureTime;
            var requestArrival = request.ArrivalTime;

            // Check for conflicts - ONLY same aircraft/plane conflicts
            var conflicts = await query.Where(f =>
                // Plane conflict - same plane at overlapping times
                f.PlaneId == request.PlaneId &&
                ((f.DepartureTime < requestArrival && f.ArrivalTime > requestDeparture)))
                .ToListAsync();

            return conflicts;
        }

        public async Task<List<Flight>> GetFlightsByManagerId(int managerId)
        {
            return await _context.Flights
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .Include(f => f.Plane)
                .Include(f => f.Status)
                .Where(f => f.ManagerId == managerId)
                .OrderByDescending(f => f.DepartureTime)
                .ToListAsync();
        }
    }
}
