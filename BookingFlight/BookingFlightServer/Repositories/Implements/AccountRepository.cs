using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;

namespace BookingFlightServer.Repositories.Implements
{
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
		public AccountRepository(BookingFlightContext repositoryDbContext) : base(repositoryDbContext)
		{
		}

		public async Task<Account?> findByUsernameAndPassword(string? username, string password)
        {
            var findAccount = await GetByCondition(
                 account =>
                  account.Username != null && account.Password != null &&
                  username != null && password != null &&
                  account.Username.Equals(username.Trim()) && account.Password.Equals(password.Trim()) &&
                  account.StatusId == 1, (account => account.Include(account => account.Role))
                );
            return findAccount;
        }

        public async Task UpdateAccountAsync(Account account)
        {
            await Update(account);
        }

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _repositoryDbContext.Accounts.AnyAsync(a => a.Username == username);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _repositoryDbContext.Customers.AnyAsync(c => c.Email == email);
        }

        public async Task<Account> CreateAccountAsync(Account account)
        {
            await Create(account);
            return account;
        }
        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            _repositoryDbContext.Customers.Add(customer);
            await _repositoryDbContext.SaveChangesAsync();
            return customer;
        }

        // Forgot Password implementations
        public async Task<Account?> FindByEmailAsync(string email)
        {
            // Email is stored in Customer table, not Account table
            var customer = await _repositoryDbContext.Customers
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.Email == email);
            return customer?.Account;
        }

        public async Task SaveResetTokenAsync(int accountId, string token, DateTime expireTime)
        {
            // Remove existing reset tokens for this account
            var existingLogs = await _repositoryDbContext.UserLogs
                .Where(ul => ul.AccountId == accountId && ul.Detail != null && ul.Detail.StartsWith("reset_token:"))
                .ToListAsync();
            
            if (existingLogs.Any())
            {
                _repositoryDbContext.UserLogs.RemoveRange(existingLogs);
            }

            // Store reset token in User_Logs table
            // Format: "reset_token:token:expireTime"
            var detail = $"reset_token:{token}:{expireTime:yyyy-MM-dd HH:mm:ss}";
            var userLog = new UserLog
            {
                AccountId = accountId,
                Detail = detail,
                AccessTime = DateTime.Now
            };
            
            _repositoryDbContext.UserLogs.Add(userLog);
            await _repositoryDbContext.SaveChangesAsync();
        }

        public async Task<(int? accountId, DateTime? expireTime)> GetResetTokenInfoAsync(string token)
        {
            var userLog = await _repositoryDbContext.UserLogs
                .Where(ul => ul.Detail != null && ul.Detail.Contains($"reset_token:{token}:"))
                .FirstOrDefaultAsync();

            if (userLog == null || userLog.Detail == null)
                return (null, null);

            // Parse detail: "reset_token:token:yyyy-MM-dd HH:mm:ss"
            var parts = userLog.Detail.Split(':');
            if (parts.Length >= 3)
            {
                var dateTimePart = string.Join(":", parts.Skip(2));
                if (DateTime.TryParseExact(dateTimePart, "yyyy-MM-dd HH:mm:ss", null,
                    System.Globalization.DateTimeStyles.None, out DateTime expireTime))
                {
                    return (userLog.AccountId, expireTime);
                }
            }

            return (null, null);
        }

        public async Task DeleteResetTokenAsync(string token)
        {
            var userLog = await _repositoryDbContext.UserLogs
                .Where(ul => ul.Detail != null && ul.Detail.Contains($"reset_token:{token}:"))
                .FirstOrDefaultAsync();

            if (userLog != null)
            {
                _repositoryDbContext.UserLogs.Remove(userLog);
                await _repositoryDbContext.SaveChangesAsync();
            }
        }

        public async Task UpdatePasswordAsync(int accountId, string newPassword)
        {
            var account = await _repositoryDbContext.Accounts.FindAsync(accountId);
            if (account != null)
            {
                account.Password = newPassword; // Plain text as requested
                await _repositoryDbContext.SaveChangesAsync();
            }
        }

        // Email Verification implementations
        public async Task SaveEmailVerificationTokenAsync(int accountId, string token, DateTime expireTime)
        {
            // Remove existing email verification tokens for this account
            var existingLogs = await _repositoryDbContext.UserLogs
                .Where(ul => ul.AccountId == accountId && ul.Detail != null && ul.Detail.StartsWith("email_verify:"))
                .ToListAsync();
            
            if (existingLogs.Any())
            {
                _repositoryDbContext.UserLogs.RemoveRange(existingLogs);
            }

            // Store verification token in User_Logs table
            // Format: "email_verify:token:expireTime"
            var detail = $"email_verify:{token}:{expireTime:yyyy-MM-dd HH:mm:ss}";
            var userLog = new UserLog
            {
                AccountId = accountId,
                Detail = detail,
                AccessTime = DateTime.Now
            };
            
            _repositoryDbContext.UserLogs.Add(userLog);
            await _repositoryDbContext.SaveChangesAsync();
        }

        public async Task<(int? accountId, DateTime? expireTime)> GetEmailVerificationTokenInfoAsync(string token)
        {
            var userLog = await _repositoryDbContext.UserLogs
                .Where(ul => ul.Detail != null && ul.Detail.Contains($"email_verify:{token}:"))
                .FirstOrDefaultAsync();

            if (userLog == null || userLog.Detail == null)
                return (null, null);

            // Parse detail: "email_verify:token:yyyy-MM-dd HH:mm:ss"
            var parts = userLog.Detail.Split(':');
            if (parts.Length >= 3)
            {
                var dateTimePart = string.Join(":", parts.Skip(2));
                if (DateTime.TryParseExact(dateTimePart, "yyyy-MM-dd HH:mm:ss", null,
                    System.Globalization.DateTimeStyles.None, out DateTime expireTime))
                {
                    return (userLog.AccountId, expireTime);
                }
            }

            return (null, null);
        }

        public async Task DeleteEmailVerificationTokenAsync(string token)
        {
            var userLog = await _repositoryDbContext.UserLogs
                .Where(ul => ul.Detail != null && ul.Detail.Contains($"email_verify:{token}:"))
                .FirstOrDefaultAsync();

            if (userLog != null)
            {
                _repositoryDbContext.UserLogs.Remove(userLog);
                await _repositoryDbContext.SaveChangesAsync();
            }
        }

        public async Task ActivateAccountAsync(int accountId)
        {
            var account = await _repositoryDbContext.Accounts.FindAsync(accountId);
            if (account != null)
            {
                account.StatusId = 1; // Active status
                await _repositoryDbContext.SaveChangesAsync();
            }
        }

		public async Task<Account?> FindByRefreshToken(string refreshToken)
		{
			return await GetByCondition(account => account.RefreshToken != null && account.RefreshToken.Equals(refreshToken));
		}
	}
}
