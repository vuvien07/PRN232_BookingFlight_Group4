using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
    public class RoleRepository : IRoleRepository
    {
        private readonly BookingFlightContext context;

        public RoleRepository(BookingFlightContext context)
        {
            this.context = context;
        }
        public async Task<List<Role>?> GetRolesAsync()
        {

            var roles = await context.Roles.ToListAsync();
            return roles == null ? null : roles;
        }
    }
}
