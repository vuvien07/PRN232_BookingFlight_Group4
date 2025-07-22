using BookingFlightServer.Entities;

namespace BookingFlightServer.Repositories
{
	public interface ICustomerRepository
	{
		Task<Customer?> GetByUsername(string username);
	}
}
