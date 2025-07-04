﻿using BookingFlightServer.Data;
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
            // For simplicity, we'll store the reset token in Redis
            // Key: reset_token:{token}, Value: {accountId}:{expireTime}
            var key = $"reset_token:{token}";
            var value = $"{accountId}:{expireTime:yyyy-MM-dd HH:mm:ss}";

            // Set expiration for 15 minutes
            var expiration = TimeSpan.FromMinutes(15);
            //await _redisHelper.SetAsync(key, value, expiration);
            // => TO DO
        }

        public async Task<(int? accountId, DateTime? expireTime)> GetResetTokenInfoAsync(string token)
        {
            var key = $"reset_token:{token}";
            //var value = await _redisHelper.GetAsync(key);
            //var value = await _redisHelper.GetAsync(key);

            //if (string.IsNullOrEmpty(value))
            //    return (null, null);

            //var parts = value.Split(':');
            //if (parts.Length >= 2 && int.TryParse(parts[0], out int accountId))
            //{
            //    // Reconstruct datetime from parts[1] and parts[2]
            //    var dateTimePart = string.Join(":", parts.Skip(1));
            //    if (DateTime.TryParseExact(dateTimePart, "yyyy-MM-dd HH:mm:ss", null,
            //        System.Globalization.DateTimeStyles.None, out DateTime expireTime))
            //    {
            //        return (accountId, expireTime);
            //    }
            //}
            return (null, null);
        }

        public async Task DeleteResetTokenAsync(string token)
        {
            var key = $"reset_token:{token}";
            //await _redisHelper.DeleteAsync(key);
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
            // Store verification token in Redis with different prefix
            var key = $"email_verify:{token}";
            var value = $"{accountId}:{expireTime:yyyy-MM-dd HH:mm:ss}";

            // Set expiration for 24 hours (longer than password reset)
            var expiration = TimeSpan.FromHours(24);
            //await _redisHelper.SetAsync(key, value, expiration);
        }

        public async Task<(int? accountId, DateTime? expireTime)> GetEmailVerificationTokenInfoAsync(string token)
        {
            //var key = $"email_verify:{token}";
            //var value = await _redisHelper.GetAsync(key);

            //if (string.IsNullOrEmpty(value))
            //    return (null, null);

            //var parts = value.Split(':');
            //if (parts.Length >= 2 && int.TryParse(parts[0], out int accountId))
            //{
            //    // Reconstruct datetime from parts[1] and parts[2]
            //    var dateTimePart = string.Join(":", parts.Skip(1));
            //    if (DateTime.TryParseExact(dateTimePart, "yyyy-MM-dd HH:mm:ss", null,
            //        System.Globalization.DateTimeStyles.None, out DateTime expireTime))
            //    {
            //        return (accountId, expireTime);
            //    }
            //}

            return (null, null);
        }

        public async Task DeleteEmailVerificationTokenAsync(string token)
        {
            var key = $"email_verify:{token}";
            //await _redisHelper.DeleteAsync(key);
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
