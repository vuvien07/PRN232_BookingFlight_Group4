using BookingFlightServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService roleService;

        public RoleController(IRoleService roleService)
        {
            this.roleService = roleService;
        }
        [HttpGet]
        [Route("get-roles")]
        public async Task<IActionResult> GetRoles()
        {
            // get
            var roles = await roleService.GetRolesAsync();
            // Check if roles is null or empty
            if (roles == null || !roles.Any())
            {
                return NotFound(new { message = "No roles found." });
            }
            return Ok(roles);
        }
    }
}
