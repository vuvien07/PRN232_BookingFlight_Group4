using BookingFlightServer.Entities;

namespace BookingFlightServer.Repositories
{
    public interface IManageAccountRepository
    {
        Task<List<Account>?> GetAccountsAsync();
        Task<Account?> CreateAccountAsync(Account account);
        Task<bool> BanAccountAsync(int accountId);
        Task<bool> UnBanAccountAsync(int accountId);
    }
}
