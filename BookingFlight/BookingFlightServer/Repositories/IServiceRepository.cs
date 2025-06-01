using BookingFlightServer.Entiies;

namespace BookingFlightServer.Repositories
{
	public interface IServiceRepository
	{
		Task<List<Service>> GetServicesByFlightId(int flightId);
	}
}
