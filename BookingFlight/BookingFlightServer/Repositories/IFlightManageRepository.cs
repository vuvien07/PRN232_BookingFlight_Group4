using BookingFlightServer.Entities;
using BookingFlightServer.DTO.Manager;

namespace BookingFlightServer.Repositories
{
    public interface IFlightManageRepository
    {
        Task<List<Flight>> GetFlightsByFilter(FlightListRequestDTO request);
        Task<int> GetTotalFlightsCount(FlightListRequestDTO request);
        Task<Flight?> GetFlightById(int flightId);
        Task<Flight> CreateFlight(Flight flight);
        Task<Flight> UpdateFlight(Flight flight);
        Task<bool> DeleteFlight(int flightId);
        Task<bool> IsFlightCodeExists(string flightCode, int? excludeFlightId = null);
        Task<List<Flight>> CheckFlightConflicts(FlightConflictCheckRequestDTO request);
        Task<List<Flight>> GetFlightsByManagerId(int managerId);
    }
}
