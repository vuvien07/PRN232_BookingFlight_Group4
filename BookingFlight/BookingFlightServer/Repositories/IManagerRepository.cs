using BookingFlightServer.Entities;

namespace BookingFlightServer.Repositories
{
    public interface IManagerRepository
    {
        Task<Account> GetAccountByUsernameAsync(string username);
        Task<Customer> GetCustomerByAccountIdAsync(int accountId);
        Task<bool> UpdateCustomerAsync(Customer customer);
        Task<Account> ValidatePasswordAsync(string username, string password);
        Task<bool> UpdatePasswordAsync(int accountId, string newPassword);
    }
}
