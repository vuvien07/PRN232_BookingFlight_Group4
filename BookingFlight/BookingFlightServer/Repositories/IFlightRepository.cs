using BookingFlightServer.DTO.Filter;
using BookingFlightServer.DTO.Query;

namespace BookingFlightServer.Repositories
{
	public interface IFlightRepository
	{
		Task<List<dynamic>> GetFlights(FilterFlightDTO filterFlightDTO);
		Task<long> GetTotalFlight(FilterFlightDTO filterFlightDTO);
		Task<List<FlightQueryDTO>> GetAllFlights();
		Task<List<FlightQueryDTO>> GetAllFlightsWithGemini(FilterFlightDTO filterFlightDTO);
	}
}
