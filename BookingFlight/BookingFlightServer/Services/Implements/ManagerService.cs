using BookingFlightServer.DTO.Manager;
using BookingFlightServer.Repositories;

namespace BookingFlightServer.Services.Implements
{
    public class ManagerService : IManagerService
    {
        private readonly IManagerRepository _managerRepository;

        public ManagerService(IManagerRepository managerRepository)
        {
            _managerRepository = managerRepository;
        }

        public async Task<ProfileResponseDTO> GetProfileByUsernameAsync(string username)
        {
            var account = await _managerRepository.GetAccountByUsernameAsync(username);
            if (account == null)
                throw new Exception("Account not found");

            var customer = await _managerRepository.GetCustomerByAccountIdAsync(account.AccountId);

            return new ProfileResponseDTO
            {
                Username = account.Username,
                FullName = customer?.Fullname ?? "",
                Address = customer?.Address ?? "",
                PhoneNumber = customer?.PhoneNumber ?? "",
                Email = customer?.Email ?? "",
                Role = account.Role?.RoleName ?? "",
                Status= account.Status?.StatusName ?? ""

            };
        }

        public async Task<ProfileResponseDTO> UpdateProfileAsync(string username, UpdateProfileRequestDTO request)
        {
            var account = await _managerRepository.GetAccountByUsernameAsync(username);
            if (account == null)
                throw new Exception("Account not found");

            var customer = await _managerRepository.GetCustomerByAccountIdAsync(account.AccountId);
            if (customer != null)
            {
                customer.Fullname = request.FullName;
                customer.Address = request.Address;
                customer.PhoneNumber = request.PhoneNumber;
                customer.Email = request.Email;

                var updateResult = await _managerRepository.UpdateCustomerAsync(customer);
                if (!updateResult)
                    throw new Exception("Failed to update profile");
            }

            return await GetProfileByUsernameAsync(username);
        }

        public async Task<bool> ChangePasswordAsync(string username, string currentPassword, string newPassword)
        {
            try
            {
                // Verify current password
                var account = await _managerRepository.ValidatePasswordAsync(username, currentPassword);
                if (account == null)
                    return false; // Current password is incorrect

                // Update to new password
                var result = await _managerRepository.UpdatePasswordAsync(account.AccountId, newPassword);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing password: {ex.Message}");
                return false;
            }
        }
    }
}