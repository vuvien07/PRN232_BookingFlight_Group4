using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using BookingFlightServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
    public class PlaneRepository : BaseRepository<Plane>, IPlaneRepository
    {
        public PlaneRepository(BookingFlightContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Plane>> GetPlanesByManagerIdAsync(int managerId)
        {
            return await FindByCondition(p => p.ManagerId == managerId, 
                query => query.Include(p => p.Manager)
                             .Include(p => p.Status))
                .OrderBy(p => p.PlaneCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<Plane>> GetPlanesWithFiltersAsync(string? search, int? statusId, int? managerId, int page, int pageSize)
        {
            var query = FindAll(query => query.Include(p => p.Manager)
                                             .Include(p => p.Status));

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.PlaneCode.ToLower().Contains(search.ToLower()));
            }

            if (statusId.HasValue)
            {
                query = query.Where(p => p.StatusId == statusId.Value);
            }

            if (managerId.HasValue)
            {
                query = query.Where(p => p.ManagerId == managerId.Value);
            }

            var result = await query
                .OrderBy(p => p.PlaneCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return result;
        }

        public async Task<int> GetTotalPlanesCountAsync(string? search, int? statusId, int? managerId)
        {
            var query = FindAll();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.PlaneCode.ToLower().Contains(search.ToLower()));
            }

            if (statusId.HasValue)
            {
                query = query.Where(p => p.StatusId == statusId.Value);
            }

            if (managerId.HasValue)
            {
                query = query.Where(p => p.ManagerId == managerId.Value);
            }

            return await query.CountAsync();
        }

        public async Task<Plane?> GetPlaneByIdAsync(int planeId)
        {
            return await GetByCondition(p => p.PlaneId == planeId,
                query => query.Include(p => p.Manager)
                             .Include(p => p.Status));
        }

        public async Task<Plane?> GetPlaneByIdForUpdateAsync(int planeId)
        {
            return await FindByCondition(p => p.PlaneId == planeId,
                query => query.Include(p => p.Manager)
                             .Include(p => p.Status))
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsPlaneCodeExistsAsync(string planeCode, int? excludePlaneId = null)
        {
            var query = FindAll().Where(p => p.PlaneCode == planeCode);
            
            if (excludePlaneId.HasValue)
            {
                query = query.Where(p => p.PlaneId != excludePlaneId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task AddAsync(Plane plane)
        {
            await Create(plane);
        }

        public async Task UpdateAsync(Plane plane)
        {
            await Update(plane);
        }

        public async Task DeleteAsync(Plane plane)
        {
            await Delete(plane);
        }
    }
}
