using BookingFlightServer.Entities;

namespace BookingFlightServer.Repositories
{
    public interface IRoleRepository
    {
        Task<List<Role>?> GetRolesAsync();
    }
}
