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

        public async Task<bool> BanAccountAsync(int accountId)
        {
            var account = await bookingFlightContext.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
            if (account == null)
            {
                return false;
            }
            account.StatusId = 2; // Assuming 2 is the ID for "Inactive" status
            bookingFlightContext.Accounts.Update(account);
            await bookingFlightContext.SaveChangesAsync();
            return true;
        }

        public async Task<Account?> CreateAccountAsync(Account account)
        {
            bookingFlightContext.Accounts.Add(account);
            await bookingFlightContext.SaveChangesAsync();
            return account == null ? null : account;
        }

        public async Task<List<Account>?> GetAccountsAsync()
        {
            var accounts = await bookingFlightContext.Accounts.Include(a => a.Role)
                                                        .Include(a => a.Status)
                                                        .ToListAsync();
            return accounts == null ? null : accounts;
        }

        public async Task<bool> UnBanAccountAsync(int accountId)
        {
            var account = await bookingFlightContext.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
            if (account == null)
            {
                return false;
            }
            account.StatusId = 1; // Assuming 1 is the ID for "Active" status
            bookingFlightContext.Accounts.Update(account);
            await bookingFlightContext.SaveChangesAsync();
            return true;
        }
    }
}
