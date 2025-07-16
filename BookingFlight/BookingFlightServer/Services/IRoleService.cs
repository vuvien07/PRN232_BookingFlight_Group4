using BookingFlightServer.DTO.Shared;

namespace BookingFlightServer.Services
{
    public interface IRoleService
    {
        Task<List<RoleDTO>?> GetRolesAsync();
    }
}
