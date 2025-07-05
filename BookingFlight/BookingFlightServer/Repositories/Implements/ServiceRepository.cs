using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using BookingFlightServer.DTO.Manager;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
	public class ServiceRepository : IServiceRepository
	{
		private BookingFlightContext _flightContext;

		public ServiceRepository(BookingFlightContext flightContext)
		{
			_flightContext = flightContext;
		}

		public async Task<List<Service>> GetServicesByFlightId(int flightId)
		{
			var queryList = _flightContext.Services.Include(s => s.Flights).Include(s => s.Items).AsAsyncEnumerable();
			List<Service> services = await queryList.Where(s => s.Flights.Any(f => f.FlightId == flightId)).ToListAsync();

			return services;
		}

		public async Task<List<Service>> GetServicesByManagerId(int managerId)
		{
			return await _flightContext.Services
				.Include(s => s.Manager)
				.Include(s => s.Status)
				.Include(s => s.Flights)
				.Include(s => s.Items)
				.Where(s => s.ManagerId == managerId)
				.ToListAsync();
		}

		public async Task<List<Service>> GetAllServices()
		{
			return await _flightContext.Services
				.Include(s => s.Manager)
				.Include(s => s.Status)
				.Include(s => s.Flights)
				.Include(s => s.Items)
				.ToListAsync();
		}

		public async Task<List<Service>> GetServicesByFilter(ServiceListRequestDTO request)
		{
			var query = _flightContext.Services
				.Include(s => s.Manager)
				.Include(s => s.Status)
				.Include(s => s.Flights)
				.Include(s => s.Items)
				.AsQueryable();

			if (!string.IsNullOrEmpty(request.SearchTerm))
			{
				query = query.Where(s => s.ServiceName.Contains(request.SearchTerm) || 
					(s.Detail != null && s.Detail.Contains(request.SearchTerm)));
			}

			if (request.StatusId.HasValue)
			{
				query = query.Where(s => s.StatusId == request.StatusId);
			}

			if (request.ManagerId.HasValue)
			{
				query = query.Where(s => s.ManagerId == request.ManagerId);
			}

			return await query
				.OrderBy(s => s.ServiceName)
				.Skip((request.Page - 1) * request.PageSize)
				.Take(request.PageSize)
				.ToListAsync();
		}

		public async Task<int> GetTotalServicesCount(ServiceListRequestDTO request)
		{
			var query = _flightContext.Services.AsQueryable();

			if (!string.IsNullOrEmpty(request.SearchTerm))
			{
				query = query.Where(s => s.ServiceName.Contains(request.SearchTerm) || 
					(s.Detail != null && s.Detail.Contains(request.SearchTerm)));
			}

			if (request.StatusId.HasValue)
			{
				query = query.Where(s => s.StatusId == request.StatusId);
			}

			if (request.ManagerId.HasValue)
			{
				query = query.Where(s => s.ManagerId == request.ManagerId);
			}

			return await query.CountAsync();
		}

		public async Task<Service?> GetServiceById(int serviceId)
		{
			return await _flightContext.Services
				.Include(s => s.Manager)
				.Include(s => s.Status)
				.Include(s => s.Flights)
				.Include(s => s.Items)
				.FirstOrDefaultAsync(s => s.ServiceId == serviceId);
		}

		public async Task<Service> CreateService(Service service)
		{
			_flightContext.Services.Add(service);
			await _flightContext.SaveChangesAsync();
			return service;
		}

		public async Task<Service> UpdateService(Service service)
		{
			_flightContext.Services.Update(service);
			await _flightContext.SaveChangesAsync();
			return service;
		}

		public async Task<bool> DeleteService(int serviceId)
		{
			var service = await _flightContext.Services.FindAsync(serviceId);
			if (service == null)
				return false;

			_flightContext.Services.Remove(service);
			await _flightContext.SaveChangesAsync();
			return true;
		}
	}
}
