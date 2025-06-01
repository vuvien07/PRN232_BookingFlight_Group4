using BookingFlightServer.Entiies;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
	public class ServiceRepository : IServiceRepository
	{
		private BookingFlightContext _flightContext;

		public ServiceRepository(BookingFlightContext flightContext)
		{
			_flightContext = flightContext;
		}
		public async Task<List<Service>> GetServicesByFlightId(int flightId)
		{
			var queryList = _flightContext.Services.Include(s => s.Flights).Include(s => s.Items).AsAsyncEnumerable();
			List<Service> services = await queryList.Where(s => s.Flights.Any(f => f.FlightId == flightId)).ToListAsync();

			return services;
		}
	}
}
