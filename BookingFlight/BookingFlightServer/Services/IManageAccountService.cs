using BookingFlightServer.DTO.ManageAccount;
using BookingFlightServer.DTO.Shared;
using BookingFlightServer.Entities;

namespace BookingFlightServer.Services
{
    public interface IManageAccountService
    {
        Task<List<ResponseAccountDTO>?> GetAccountsAsync();
        Task<ResponseAccountDTO?> CreateAccountAsync(RequestAddAccountDTO requestAddAccountDTO);
    }
}
