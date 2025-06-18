
using BookingFlightServer.Entities;

namespace BookingFlightClient.Repositories
{
	public interface IPermissionApiRepository
	{
		Task<PermissionApi?> GetPermissionByRoleAndUrl(string role, string url);
		Task<List<PermissionApi>> GetPermissionByRole(string role);
	}
}
