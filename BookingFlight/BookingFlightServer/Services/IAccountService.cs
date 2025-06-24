using BookingFlightServer.Entities;

namespace BookingFlightServer.Services
{
	public interface IAccountService
	{
		Task<Account?> findByUsernameAndPassword(string? username, string? password);
		Task UpdateAccountAsync(Account account);
        Task<bool> IsUsernameExistsAsync(string username);
        Task<bool> IsEmailExistsAsync(string email);
        Task<Customer> RegisterCustomerAsync(string username, string password, string fullname, string address, string phoneNumber, string email);
        
        // Forgot Password methods
        Task<bool> SendForgotPasswordEmailAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
        
        // Email Verification methods
        Task<bool> SendEmailVerificationAsync(string email, int accountId);
        Task<bool> VerifyEmailAsync(string token);
	}
}
