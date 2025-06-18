using BookingFlightClient.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightClient.Repositories.Implements
{
	public class PermissionPageRepository : IPermissionPageRepository
	{
		private BookingFlightContext _flightContext;

		public PermissionPageRepository(BookingFlightContext flightContext)
		{
			_flightContext = flightContext;
		}

		public async Task<List<PermissionPage>> GetListPermissionByRole(string role)
		{
			return await _flightContext.PermissionPages
				.Where(p => p.Roles.Any(p => p.RoleName.Equals(role))).ToListAsync();
		}

		public async Task<PermissionPage?> GetPermissionByRoleAndUrl(string role, string url)
		{
			var query = await _flightContext.PermissionPages
			.FirstOrDefaultAsync(p => p.Url != null && p.Url.Equals(url) && p.Roles.Any(r => r.RoleName.Equals(role)));
			return query;
		}
	}
}
