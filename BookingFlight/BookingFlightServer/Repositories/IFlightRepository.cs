using BookingFlightServer.DTO.Filter;

namespace BookingFlightServer.Repositories
{
	public interface IFlightRepository
	{
		Task<List<dynamic>> GetFlights(FilterFlightDTO filterFlightDTO);
		Task<long> GetTotalFlight(FilterFlightDTO filterFlightDTO);
	}
}
