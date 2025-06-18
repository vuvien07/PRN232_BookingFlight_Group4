
using BookingFlightServer.Entities;

namespace BookingFlightServer.Repositories
{
	public interface IServiceRepository
	{
		Task<List<Service>> GetServicesByFlightId(int flightId);
	}
}
