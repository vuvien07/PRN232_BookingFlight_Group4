using BookingFlightServer.DTO.Shared;
using BookingFlightServer.Repositories;

namespace BookingFlightServer.Services.Implements
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            this.roleRepository = roleRepository;
        }
        public async Task<List<RoleDTO>?> GetRolesAsync()
        {
            var roles = await roleRepository.GetRolesAsync();
            // If roles is null, return null
            if (roles == null)
            {
                return null;
            }
            // Map roles to RoleDTOs
            var roleDTOs = roles.Select(role => new RoleDTO
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName
            }).ToList();

            return roleDTOs;
        }
    }
}
