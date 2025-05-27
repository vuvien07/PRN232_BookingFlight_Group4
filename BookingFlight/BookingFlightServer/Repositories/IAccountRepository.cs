using BookingFlightServer.Entiies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> findByUsernameAndPassword(string? username, string? password);
		Task UpdateAccountAsync(Account account);
	}
}
