using BookingFlightServer.DTO.Manager;
using BookingFlightServer.DTO.Shared;
using BookingFlightServer.Entities;
using BookingFlightServer.Repositories;
using BookingFlightServer.Data;
using Microsoft.EntityFrameworkCore;
using ManagerItemDTO = BookingFlightServer.DTO.Manager.ItemDTO;

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

        public async Task<ServiceDetailsDTO?> GetServiceDetails(int serviceId)
        {
            var service = await _serviceRepository.GetServiceById(serviceId);
            if (service == null) return null;

            return new ServiceDetailsDTO
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
                Items = service.Items?.Select(i => new ManagerItemDTO
                {
                    ItemId = i.ItemId,
                    ItemName = i.ItemName,
                    Detail = i.Detail,
                    Price = i.Price,
                    StatusId = i.StatusId,
                    StatusName = i.Status?.StatusName,
                    Image = i.Image
                }).ToList() ?? new List<ManagerItemDTO>(),
                FlightIds = service.Flights?.Select(f => f.FlightId).ToList() ?? new List<int>()
            };
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

            // Add items to service if provided
            if (request.ItemIds != null && request.ItemIds.Any())
            {
                foreach (var itemId in request.ItemIds)
                {
                    // Add to Service_Item table
                    var item = await _context.Items.FindAsync(itemId);
                    if (item != null)
                    {
                        createdService.Items.Add(item);
                    }
                }
                await _context.SaveChangesAsync();
            }

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

        public async Task<ServiceDetailsDTO> UpdateServiceAdvanced(ServiceUpdateAdvancedRequestDTO request)
        {
            var existingService = await _serviceRepository.GetServiceById(request.ServiceId);
            if (existingService == null)
                throw new InvalidOperationException("Service not found");

            // Update service basic info
            existingService.ServiceName = request.ServiceName;
            existingService.Detail = request.Detail;
            existingService.StatusId = request.StatusId;

            // Remove items
            if (request.ItemIdsToRemove.Any())
            {
                var itemsToRemove = existingService.Items.Where(i => request.ItemIdsToRemove.Contains(i.ItemId)).ToList();
                foreach (var item in itemsToRemove)
                {
                    existingService.Items.Remove(item);
                }
            }

            // Update existing items
            foreach (var itemUpdate in request.Items)
            {
                var existingItem = await _context.Items.FindAsync(itemUpdate.ItemId);
                if (existingItem != null)
                {
                    existingItem.ItemName = itemUpdate.ItemName;
                    existingItem.Detail = itemUpdate.Detail;
                    existingItem.Price = itemUpdate.Price;
                    existingItem.StatusId = itemUpdate.StatusId;
                    existingItem.Image = itemUpdate.Image;
                }
            }

            // Add new items
            foreach (var newItemRequest in request.NewItems)
            {
                var newItem = new Entities.Item
                {
                    ItemName = newItemRequest.ItemName,
                    Detail = newItemRequest.Detail,
                    Price = newItemRequest.Price,
                    StatusId = newItemRequest.StatusId,
                    Image = newItemRequest.Image
                };

                _context.Items.Add(newItem);
                await _context.SaveChangesAsync();
                existingService.Items.Add(newItem);
            }

            await _serviceRepository.UpdateService(existingService);
            await _context.SaveChangesAsync();

            return await GetServiceDetails(request.ServiceId) ?? throw new InvalidOperationException("Failed to get updated service details");
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
