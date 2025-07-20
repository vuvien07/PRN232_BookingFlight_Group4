using BookingFlightServer.DTO.Profile;

namespace BookingFlightServer.Services
{
    public interface IProfileService
    {
        Task<ProfileResponseDTO> GetProfileByUsernameAsync(string username);
        Task<ProfileResponseDTO> UpdateProfileAsync(string username, UpdateProfileRequestDTO request);
        Task<bool> ChangePasswordAsync(string username, string currentPassword, string newPassword);
    }
}