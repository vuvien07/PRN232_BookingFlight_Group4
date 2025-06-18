using BookingFlightServer.Entities;
using Repositories;

namespace BookingFlightServer.Services.Implements
{
	public class AccountService : IAccountService
	{
		private readonly IAccountRepository _accountRepository;

		public AccountService(IAccountRepository accountRepository)
		{
			_accountRepository = accountRepository;
		}
		public async Task<Account?> findByUsernameAndPassword(string? username, string? password)
		{
			return await _accountRepository.findByUsernameAndPassword(username, password);
		}

		public async Task UpdateAccountAsync(Account account)
		{
			await _accountRepository.UpdateAccountAsync(account);
		}
	}
}
