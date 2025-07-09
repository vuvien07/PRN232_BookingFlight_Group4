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
                Fullname = a.Customer?.Fullname,
                Address = a.Customer?.Address,
                PhoneNumber = a.Customer?.PhoneNumber,
                Email = a.Customer?.Email,
                Role = a.Role.RoleName,
                Status = a.Status.StatusName
            }).ToList();

            return responseAccountDTO;
        }

        public async Task<bool> UpdateAccountAsync(string username, ResponseAccountDTO updateData)
        {
            try
            {
                var account = await manageAccountRepository.GetAccountByUsernameAsync(username);
                if (account == null)
                {
                    return false;
                }

                // Update customer information
                if (account.Customer != null)
                {
                    account.Customer.Fullname = updateData.Fullname ?? account.Customer.Fullname;
                    account.Customer.Address = updateData.Address ?? account.Customer.Address;
                    account.Customer.PhoneNumber = updateData.PhoneNumber ?? account.Customer.PhoneNumber;
                    account.Customer.Email = updateData.Email ?? account.Customer.Email;
                }

                await manageAccountRepository.UpdateAccountAsync(account);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
