using BookingFlightServer.DTO;

namespace BookingFlightServer.Services
{
	public interface IFlightService
	{
		Task<List<dynamic>> GetFlights(FilterFlightDTO filterFlightDTO);
		Task<long> GetTotalFlight(FilterFlightDTO filterFlightDTO);
		Task<List<dynamic>> GetAllClassSeatByFlightIdAndSeatEmpty(int flightId);
		Task<long> CountAllClassSeatByFlightIdAndSeatEmpty(int flightId);
	}
}
