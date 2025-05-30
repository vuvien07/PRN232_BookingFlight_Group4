using BookingFlightServer.DTO;
using BookingFlightServer.Repositories;

namespace BookingFlightServer.Services.Implements
{
	public class FlightService : IFlightService
	{
		private readonly IFlightRepository _flightRepository;
		private readonly IClassSeatRepository _classSeatRepository;

		public FlightService(IFlightRepository flightRepository, IClassSeatRepository classSeatRepository)
		{
			_flightRepository = flightRepository;
			_classSeatRepository = classSeatRepository;
		}

		public async Task<long> CountAllClassSeatByFlightIdAndSeatEmpty(int flightId)
		{
			return await _classSeatRepository.CountAllClassSeatByFlightIdAndSeatEmpty(flightId);
		}

		public async Task<List<dynamic>> GetAllClassSeatByFlightIdAndSeatEmpty(int flightId)
		{
			return await _classSeatRepository.GetAllClassSeatByFlightIdAndSeatEmpty(flightId);
		}

		public async Task<List<dynamic>> GetFlights(FilterFlightDTO filterFlightDTO)
		{
			return await _flightRepository.GetFlights(filterFlightDTO);
		}

		public async Task<long> GetTotalFlight(FilterFlightDTO filterFlightDTO)
		{
			return await _flightRepository.GetTotalFlight(filterFlightDTO);
		}
	}
}
