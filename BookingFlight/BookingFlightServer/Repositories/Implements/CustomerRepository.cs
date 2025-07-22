using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
	public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
	{
		public CustomerRepository(BookingFlightContext repositoryDbContext) : base(repositoryDbContext)
		{
		}

		public async Task<Customer?> GetByUsername(string username)
		{
			return await GetByCondition(c => c.Account.Username == username.Trim(),
				c => c.Include(c => c.Account));
		}
	}
}
