using BookingFlightClient.Repositories;
using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
	public class PermissionApiRepository : IPermissionApiRepository
	{
		private BookingFlightContext _flightContext;

		public PermissionApiRepository(BookingFlightContext flightContext)
		{
			_flightContext = flightContext;
		}

		public async Task<List<PermissionApi>> GetPermissionByRole(string role)
		{
			return await _flightContext.PermissionApis.Where(p => p.Roles.Any(r => r.RoleName.Equals(role))).ToListAsync();
		}

		public async Task<PermissionApi?> GetPermissionByRoleAndUrl(string role, string url)
		{
			var query = await _flightContext.PermissionApis
			.FirstOrDefaultAsync(p => p.Url != null && p.Url.Equals(url) && p.Roles.Any(r => r.RoleName.Equals(role)));
			return query;
		}
	}
}
