using BookingFlightServer.DTO.ManageAccount;
using BookingFlightServer.Entities;

namespace BookingFlightServer.Services
{
    public interface IManageAccountService
    {
        Task<List<ResponseAccountDTO>?> GetAccountsAsync();
    }
}
