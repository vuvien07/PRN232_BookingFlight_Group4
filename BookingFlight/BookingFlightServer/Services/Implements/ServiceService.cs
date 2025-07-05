using BookingFlightServer.DTO.Manager;
using BookingFlightServer.DTO.Shared;
using BookingFlightServer.Entities;
using BookingFlightServer.Repositories;
using BookingFlightServer.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Services.Implements
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly BookingFlightContext _context;

        public ServiceService(
            IServiceRepository serviceRepository,
            BookingFlightContext context)
        {
            _serviceRepository = serviceRepository;
            _context = context;
        }

        public async Task<List<DTO.Manager.ServiceDTO>> GetServicesByManagerId(int managerId)
        {
            var services = await _serviceRepository.GetServicesByManagerId(managerId);
            return services.Select(MapToDTO).ToList();
        }

        public async Task<List<DTO.Manager.ServiceDTO>> GetAllServices()
        {
            var services = await _serviceRepository.GetAllServices();
            return services.Select(MapToDTO).ToList();
        }

        public async Task<List<DTO.Manager.ServiceDTO>> GetServicesByFilter(ServiceListRequestDTO request)
        {
            var services = await _serviceRepository.GetServicesByFilter(request);
            return services.Select(MapToDTO).ToList();
        }

        public async Task<int> GetTotalServicesCount(ServiceListRequestDTO request)
        {
            return await _serviceRepository.GetTotalServicesCount(request);
        }

        public async Task<DTO.Manager.ServiceDTO?> GetServiceById(int serviceId)
        {
            var service = await _serviceRepository.GetServiceById(serviceId);
            return service != null ? MapToDTO(service) : null;
        }

        public async Task<DTO.Manager.ServiceDTO> CreateService(ServiceCreateRequestDTO request)
        {
            var service = new Service
            {
                ServiceName = request.ServiceName,
                Detail = request.Detail,
                ManagerId = request.ManagerId,
                StatusId = request.StatusId
            };

            var createdService = await _serviceRepository.CreateService(service);
            var serviceWithIncludes = await _serviceRepository.GetServiceById(createdService.ServiceId);
            return MapToDTO(serviceWithIncludes!);
        }

        public async Task<DTO.Manager.ServiceDTO> UpdateService(ServiceUpdateRequestDTO request)
        {
            var existingService = await _serviceRepository.GetServiceById(request.ServiceId);
            if (existingService == null)
                throw new InvalidOperationException("Service not found");

            existingService.ServiceName = request.ServiceName;
            existingService.Detail = request.Detail;
            existingService.StatusId = request.StatusId;

            var updatedService = await _serviceRepository.UpdateService(existingService);
            var serviceWithIncludes = await _serviceRepository.GetServiceById(updatedService.ServiceId);
            return MapToDTO(serviceWithIncludes!);
        }

        public async Task<bool> DeleteService(int serviceId)
        {
            return await _serviceRepository.DeleteService(serviceId);
        }

        public async Task<List<StatusDTO>> GetServiceStatuses()
        {
            var statuses = await _context.Statuses.Where(s => s.StatusType == "Service").ToListAsync();
            return statuses.Select(s => new StatusDTO
            {
                StatusId = s.StatusId,
                StatusName = s.StatusName,
                StatusType = s.StatusType
            }).ToList();
        }

        private DTO.Manager.ServiceDTO MapToDTO(Service service)
        {
            return new DTO.Manager.ServiceDTO
            {
                ServiceId = service.ServiceId,
                ServiceName = service.ServiceName,
                Detail = service.Detail,
                ManagerId = service.ManagerId,
                StatusId = service.StatusId,
                Status = service.Status != null ? new StatusDTO
                {
                    StatusId = service.Status.StatusId,
                    StatusName = service.Status.StatusName,
                    StatusType = service.Status.StatusType
                } : null,
                ManagerName = service.Manager?.Fullname ?? "Unknown",
                FlightCount = service.Flights?.Count ?? 0,
                ItemCount = service.Items?.Count ?? 0
            };
        }
    }
}
