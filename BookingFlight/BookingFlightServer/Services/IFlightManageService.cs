using BookingFlightServer.DTO.Manager;
using BookingFlightServer.DTO.Shared;

namespace BookingFlightServer.Services
{
    public interface IFlightManageService
    {
        Task<List<FlightManageDTO>> GetFlightsByFilter(FlightListRequestDTO request);
        Task<int> GetTotalFlightsCount(FlightListRequestDTO request);
        Task<FlightManageDTO?> GetFlightById(int flightId);
        Task<FlightManageDTO> CreateFlight(FlightCreateRequestDTO request, int managerId);
        Task<FlightManageDTO> UpdateFlight(FlightUpdateRequestDTO request, int managerId);
        Task<bool> DeleteFlight(int flightId);
        Task<FlightConflictResultDTO> CheckFlightConflicts(FlightConflictCheckRequestDTO request);
        Task<List<FlightManageDTO>> GetFlightsByManagerId(int managerId);
        Task<List<StatusDTO>> GetFlightStatuses();
    }
}
