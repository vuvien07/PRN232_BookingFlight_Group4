using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
    public class ManageAccountRepository : IManageAccountRepository
    {
        private readonly BookingFlightContext bookingFlightContext;

        public ManageAccountRepository(BookingFlightContext bookingFlightContext)
        {
            this.bookingFlightContext = bookingFlightContext;
        }
        public async Task<List<Account>?> GetAccountsAsync()
        {
            var accounts = await bookingFlightContext.Accounts.Include(a => a.Role)
                                                        .Include(a => a.Status)
                                                        .Include(a => a.Customer)
                                                        .ToListAsync();
            return accounts == null ? null : accounts;
        }

        public async Task<Account?> GetAccountByUsernameAsync(string username)
        {
            var account = await bookingFlightContext.Accounts
                .Include(a => a.Role)
                .Include(a => a.Status)
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Username == username);
            return account;
        }

        public async Task UpdateAccountAsync(Account account)
        {
            bookingFlightContext.Accounts.Update(account);
            await bookingFlightContext.SaveChangesAsync();
        }
    }
}
