using AutoMapper;
using BookingFlightServer.DTO.ManageAccount;
using BookingFlightServer.DTO.Shared;
using BookingFlightServer.Entities;
using BookingFlightServer.Repositories;

namespace BookingFlightServer.Services.Implements
{
    public class ManageAccountService : IManageAccountService
    {
        private readonly IManageAccountRepository manageAccountRepository;
        private readonly IMapper mapper;

        public ManageAccountService(IManageAccountRepository manageAccountRepository)
        {
            this.manageAccountRepository = manageAccountRepository;
        }

        public async Task<ResponseAccountDTO?> CreateAccountAsync(RequestAddAccountDTO requestAddAccountDTO)
        {
            // Convert RequestAddAccountDTO to Account entity
            var account = new Account
            {
                Username = requestAddAccountDTO.Username,
                Password = requestAddAccountDTO.Password,
                RoleId = requestAddAccountDTO.RoleId,
                StatusId = requestAddAccountDTO.StatusId
            };
            // Call the repository to create the account
            var createdAccount = await manageAccountRepository.CreateAccountAsync(account);
            // Check if the account was created successfully
            return createdAccount == null ? null : new ResponseAccountDTO
            {
                AccountId = createdAccount.AccountId,
                Username = createdAccount.Username,
                Password = createdAccount.Password,
                RoleId = createdAccount.RoleId,
                StatusId = createdAccount.StatusId
            };
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
