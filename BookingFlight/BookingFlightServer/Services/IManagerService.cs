using BookingFlightServer.DTO.Manager;

namespace BookingFlightServer.Services
{
    public interface IManagerService
    {
        Task<ProfileResponseDTO> GetProfileByUsernameAsync(string username);
        Task<ProfileResponseDTO> UpdateProfileAsync(string username, UpdateProfileRequestDTO request);
        Task<bool> ChangePasswordAsync(string username, string currentPassword, string newPassword);
    }
}