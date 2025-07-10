using BookingFlightServer.Entities;
using Repositories;

namespace BookingFlightServer.Services.Implements
{	public class AccountService : IAccountService
	{
		private readonly IAccountRepository _accountRepository;
		private readonly IEmailService _emailService;

		public AccountService(IAccountRepository accountRepository, IEmailService emailService)
		{
			_accountRepository = accountRepository;
			_emailService = emailService;
		}
		public async Task<Account?> findByUsernameAndPassword(string? username, string? password)
		{
			return await _accountRepository.findByUsernameAndPassword(username, password);
		}

		public async Task UpdateAccountAsync(Account account)
		{
			await _accountRepository.UpdateAccountAsync(account);
		}

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _accountRepository.IsUsernameExistsAsync(username);
        }        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _accountRepository.IsEmailExistsAsync(email);
        }

        public async Task<Customer> RegisterCustomerAsync(string username, string password, string fullname, string address, string phoneNumber, string email)
        {
            try 
            {
                // Create Account first
                var account = new Account
                {
                    Username = username,
                    Password = password, // Store password directly as entered by user
                    RoleId = 3, // Customer role
                    StatusId = 2 // Inactive status - will be activated after email verification
                };

                var createdAccount = await _accountRepository.CreateAccountAsync(account);

                // Create Customer with the account ID
                var customer = new Customer
                {
                    Fullname = fullname,
                    Address = address,
                    PhoneNumber = phoneNumber,
                    Email = email,
                    AccountId = createdAccount.AccountId
                };

                var createdCustomer = await _accountRepository.CreateCustomerAsync(customer);

                // Send email verification
                await SendEmailVerificationAsync(email, createdAccount.AccountId);

                return createdCustomer;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RegisterCustomerAsync: {ex.Message}");
                throw;
            }
        }

        // Forgot Password implementations
        public async Task<bool> SendForgotPasswordEmailAsync(string email)
        {
            // Find account by email
            var account = await _accountRepository.FindByEmailAsync(email);
            if (account == null)
                return false;

            // Generate reset token (simple random string)
            var token = Guid.NewGuid().ToString("N")[..16]; // 16 character token
            var expireTime = DateTime.Now.AddMinutes(15); // Token expires in 15 minutes
            
            // Save token to database
            await _accountRepository.SaveResetTokenAsync(account.AccountId, token, expireTime);

            // Try to send email, fallback to console if email fails
            bool emailSent = await _emailService.SendForgotPasswordEmailAsync(email, token);
            
            if (!emailSent)
            {
                // Fallback: log to console if email sending fails
                Console.WriteLine($"Email sending failed. Reset password token for {email}: {token}");
                Console.WriteLine($"Reset link: http://localhost:5001/ForgotPassword/Reset?token={token}");
            }

            return true;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            // Get token info
            var (accountId, expireTime) = await _accountRepository.GetResetTokenInfoAsync(token);
            
            if (accountId == null || expireTime == null)
                return false; // Token not found

            if (DateTime.Now > expireTime)
            {
                // Token expired, delete it
                await _accountRepository.DeleteResetTokenAsync(token);
                return false;
            }

            // Update password
            await _accountRepository.UpdatePasswordAsync(accountId.Value, newPassword);
            
            // Delete used token
            await _accountRepository.DeleteResetTokenAsync(token);

            return true;
        }

        // Email Verification implementations
        public async Task<bool> SendEmailVerificationAsync(string email, int accountId)
        {
            try
            {
                // Generate verification token (16 character random string)
                var token = Guid.NewGuid().ToString("N")[..16];
                var expireTime = DateTime.Now.AddHours(24); // Token expires in 24 hours

                // Save token to database
                await _accountRepository.SaveEmailVerificationTokenAsync(accountId, token, expireTime);

                // Try to send email, fallback to console if email fails
                bool emailSent = await _emailService.SendEmailVerificationAsync(email, token);
                
                if (!emailSent)
                {
                    // Fallback: log to console if email sending fails
                    Console.WriteLine($"Email verification failed. Verification token for {email}: {token}");
                    Console.WriteLine($"Verification link: http://localhost:5001/Register/VerifyEmail?token={token}");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending verification email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            try
            {
                // Get token info
                var (accountId, expireTime) = await _accountRepository.GetEmailVerificationTokenInfoAsync(token);
                
                if (accountId == null || expireTime == null)
                    return false; // Token not found

                if (DateTime.Now > expireTime)
                {
                    // Token expired, delete it
                    await _accountRepository.DeleteEmailVerificationTokenAsync(token);
                    return false;
                }

                // Activate account
                await _accountRepository.ActivateAccountAsync(accountId.Value);
                
                // Delete used token
                await _accountRepository.DeleteEmailVerificationTokenAsync(token);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying email: {ex.Message}");
                return false;
            }
        }

		public async Task<Account?> FindByRefreshTokenAsync(string refreshToken)
		{
			return await _accountRepository.FindByRefreshToken(refreshToken);
		}
	}
}
