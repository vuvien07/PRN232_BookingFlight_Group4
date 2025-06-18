using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;

namespace BookingFlightServer.Repositories.Implements
{
	public class AccountRepository : IAccountRepository
	{
		private BookingFlightContext _flightContext;

		public AccountRepository(BookingFlightContext flightContext)
		{
			_flightContext = flightContext;
		}
		public async Task<Account?> findByUsernameAndPassword(string? username, string? password)
		{
			var findAccount = await _flightContext.Accounts.Include(account => account.Role)
			  .FirstOrDefaultAsync(account =>
				  account.Username != null && account.Password != null &&
				  username != null && password != null &&
				  account.Username.Equals(username.Trim()) && account.Password.Equals(password.Trim()));
			return findAccount;
		}

		public async Task UpdateAccountAsync(Account account)
		{
			_flightContext.Accounts.Update(account);
			await _flightContext.SaveChangesAsync();
		}
	}
}
