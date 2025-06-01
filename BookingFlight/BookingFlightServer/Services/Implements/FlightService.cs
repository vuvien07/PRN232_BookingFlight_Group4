using BookingFlightServer.DTO;
using BookingFlightServer.Entiies;
using BookingFlightServer.Repositories;

namespace BookingFlightServer.Services.Implements
{
	public class FlightService : IFlightService
	{
		private readonly IFlightRepository _flightRepository;
		private readonly IClassSeatRepository _classSeatRepository;
		private readonly IServiceRepository _serviceRepository;
		public FlightService(IFlightRepository flightRepository, IClassSeatRepository classSeatRepository, IServiceRepository serviceRepository)
		{
			_flightRepository = flightRepository;
			_classSeatRepository = classSeatRepository;
			_serviceRepository = serviceRepository;
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

		public async Task<List<Service>> GetServicesByFlightId(int flightId)
		{
			return await _serviceRepository.GetServicesByFlightId(flightId);
		}

		public async Task<long> GetTotalFlight(FilterFlightDTO filterFlightDTO)
		{
			return await _flightRepository.GetTotalFlight(filterFlightDTO);
		}
	}
}
