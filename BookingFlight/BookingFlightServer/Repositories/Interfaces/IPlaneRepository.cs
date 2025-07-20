using BookingFlightServer.Entities;

namespace BookingFlightServer.Repositories.Interfaces
{
    public interface IPlaneRepository : IBaseRepository<Plane>
    {
        Task<IEnumerable<Plane>> GetPlanesByManagerIdAsync(int managerId);
        Task<IEnumerable<Plane>> GetPlanesWithFiltersAsync(string? search, int? statusId, int? managerId, int page, int pageSize);
        Task<int> GetTotalPlanesCountAsync(string? search, int? statusId, int? managerId);
        Task<Plane?> GetPlaneByIdAsync(int planeId);
        Task<Plane?> GetPlaneByIdForUpdateAsync(int planeId);
        Task<bool> IsPlaneCodeExistsAsync(string planeCode, int? excludePlaneId = null);
        Task AddAsync(Plane plane);
        Task UpdateAsync(Plane plane);
        Task DeleteAsync(Plane plane);
    }
}
