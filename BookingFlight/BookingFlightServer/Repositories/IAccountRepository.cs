using BookingFlightServer.Entities;
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
		Task<Account?> FindByRefreshToken(string refreshToken);
		Task UpdateAccountAsync(Account account);
        Task<bool> IsUsernameExistsAsync(string username);
        Task<bool> IsEmailExistsAsync(string email);
        Task<Account> CreateAccountAsync(Account account);
        Task<Customer> CreateCustomerAsync(Customer customer);
        
        // Forgot Password methods
        Task<Account?> FindByEmailAsync(string email);
        Task SaveResetTokenAsync(int accountId, string token, DateTime expireTime);
        Task<(int? accountId, DateTime? expireTime)> GetResetTokenInfoAsync(string token);
        Task DeleteResetTokenAsync(string token);
        Task UpdatePasswordAsync(int accountId, string newPassword);
        
        // Email Verification methods
        Task SaveEmailVerificationTokenAsync(int accountId, string token, DateTime expireTime);
        Task<(int? accountId, DateTime? expireTime)> GetEmailVerificationTokenInfoAsync(string token);
        Task DeleteEmailVerificationTokenAsync(string token);
        Task ActivateAccountAsync(int accountId);
	}
}
