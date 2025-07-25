using BookingFlightServer.Entities;

namespace BookingFlightServer.Repositories
{
    public interface IFlightSeatRepository
    {
        // Basic operations
        Task<List<FlightSeat>> GetFlightSeatsByFlightId(int flightId);
        Task UpdateFlightSeatAsync(FlightSeat flightSeat);
        
        // New enhanced methods for manager functionality
        Task<FlightSeat?> GetFlightSeatByIdAsync(int flightId, int seatId);
        Task<bool> UpdateFlightSeatStatusAsync(int flightId, int seatId, bool isSat, int? ticketId = null);
        Task<bool> AssignSeatToTicketAsync(int flightId, int seatId, int ticketId);
        Task<bool> UnassignSeatAsync(int flightId, int seatId);
        Task<List<FlightSeat>> GetAvailableFlightSeatsAsync(int flightId);
        Task<List<FlightSeat>> GetOccupiedFlightSeatsAsync(int flightId);
        Task<int> GetAvailableSeatCountAsync(int flightId);
        Task<int> GetOccupiedSeatCountAsync(int flightId);
        Task<bool> CreateFlightSeatsForFlightAsync(int flightId);
    }
}
