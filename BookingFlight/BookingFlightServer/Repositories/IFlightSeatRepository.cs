using BookingFlightServer.Entities;

namespace BookingFlightServer.Repositories
{
	public interface IFlightSeatRepository
	{
		Task<List<FlightSeat?>> GetFlightSeatsByFlightId(int flightId);
		Task UpdateFlightSeatAsync(FlightSeat flightSeat);
		Task<FlightSeat?> GetFlightSeatByTicketId(int ticketId);
	}
}
