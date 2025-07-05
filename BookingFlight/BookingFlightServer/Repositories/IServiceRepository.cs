
using BookingFlightServer.Entities;
using BookingFlightServer.DTO.Manager;

namespace BookingFlightServer.Repositories
{
	public interface IServiceRepository
	{
		Task<List<Service>> GetServicesByFlightId(int flightId);
		Task<List<Service>> GetServicesByManagerId(int managerId);
		Task<List<Service>> GetAllServices();
		Task<List<Service>> GetServicesByFilter(ServiceListRequestDTO request);
		Task<int> GetTotalServicesCount(ServiceListRequestDTO request);
		Task<Service?> GetServiceById(int serviceId);
		Task<Service> CreateService(Service service);
		Task<Service> UpdateService(Service service);
		Task<bool> DeleteService(int serviceId);
	}
}
