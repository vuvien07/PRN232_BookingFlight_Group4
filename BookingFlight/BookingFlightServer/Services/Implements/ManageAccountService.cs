using BookingFlightServer.DTO.ManageAccount;
using BookingFlightServer.Entities;
using BookingFlightServer.Repositories;

namespace BookingFlightServer.Services.Implements
{
    public class ManageAccountService : IManageAccountService
    {
        private readonly IManageAccountRepository manageAccountRepository;

        public ManageAccountService(IManageAccountRepository manageAccountRepository)
        {
            this.manageAccountRepository = manageAccountRepository;
        }
        public async Task<List<ResponseAccountDTO>?> GetAccountsAsync()
        {
            var accounts = await manageAccountRepository.GetAccountsAsync();

            if (accounts == null)
            {
                return null;
            }
            var responseAccountDTO = accounts.Select(a => new ResponseAccountDTO
            {
                AccountId = a.AccountId,
                Username = a.Username,
                Role = a.Role.RoleName,
                Status = a.Status.StatusName
            }).ToList();

            return responseAccountDTO;
        }


    }
}
