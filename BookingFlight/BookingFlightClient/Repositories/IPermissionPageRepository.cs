using BookingFlightClient.Entities;

namespace BookingFlightClient.Repositories
{
	public interface IPermissionPageRepository
	{
		Task<PermissionPage?> GetPermissionByRoleAndUrl(string role, string url);
		Task<List<PermissionPage>> GetListPermissionByRole(string role);
	}
}
