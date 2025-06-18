
using BookingFlightServer.Entities;

namespace BookingFlightServer.Services
{
	public interface IAccountService
	{
		Task<Account?> findByUsernameAndPassword(string? username, string? password);
		Task UpdateAccountAsync(Account account);
	}
}
