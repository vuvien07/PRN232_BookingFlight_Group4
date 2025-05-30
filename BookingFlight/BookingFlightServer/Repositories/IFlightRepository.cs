using BookingFlightServer.DTO;

namespace BookingFlightServer.Repositories
{
	public interface IFlightRepository
	{
		Task<List<dynamic>> GetFlights(FilterFlightDTO filterFlightDTO);
		Task<long> GetTotalFlight(FilterFlightDTO filterFlightDTO);
	}
}
