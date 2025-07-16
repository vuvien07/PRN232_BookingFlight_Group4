using BookingFlightServer.DTO.Manager;
using BookingFlightServer.DTO.Shared;

namespace BookingFlightServer.Services
{
    public interface IServiceService
    {
        Task<List<DTO.Manager.ServiceDTO>> GetServicesByManagerId(int managerId);
        Task<List<DTO.Manager.ServiceDTO>> GetAllServices();
        Task<List<DTO.Manager.ServiceDTO>> GetServicesByFilter(ServiceListRequestDTO request);
        Task<int> GetTotalServicesCount(ServiceListRequestDTO request);
        Task<DTO.Manager.ServiceDTO?> GetServiceById(int serviceId);
        Task<ServiceDetailsDTO?> GetServiceDetails(int serviceId);
        Task<DTO.Manager.ServiceDTO> CreateService(ServiceCreateRequestDTO request);
        Task<DTO.Manager.ServiceDTO> UpdateService(ServiceUpdateRequestDTO request);
        Task<ServiceDetailsDTO> UpdateServiceAdvanced(ServiceUpdateAdvancedRequestDTO request);
        Task<bool> DeleteService(int serviceId);
        Task<List<StatusDTO>> GetServiceStatuses();
    }
}
