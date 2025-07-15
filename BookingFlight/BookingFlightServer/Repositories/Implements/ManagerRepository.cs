using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
    public class ManagerRepository : IManagerRepository
    {
        private readonly BookingFlightContext _context;
        public ManagerRepository(BookingFlightContext context)
        {
            _context = context;
        }
        public async Task<Account> GetAccountByUsernameAsync(string username)
        {
            return await _context.Accounts
                 .Include(a => a.Role)
                 .FirstOrDefaultAsync(a => a.Username == username);
        }

        public async Task<Customer> GetCustomerByAccountIdAsync(int accountId)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.AccountId == accountId);
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Account> ValidatePasswordAsync(string username, string password)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.Username == username && a.Password == password);
        }

        public async Task<bool> UpdatePasswordAsync(int accountId, string newPassword)
        {
            try
            {
                var account = await _context.Accounts.FindAsync(accountId);
                if (account == null)
                    return false;

                account.Password = newPassword;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
