using BookingFlightServer.DTO;

namespace BookingFlightServer.Services
{
	public interface ICheckoutFlightService
	{
		decimal caculateServicePrice(List<ServiceDTO> serviceDTOs);
		int caculateTotalPerson(List<PreorderFlightDTO> preorderFlightDTOs);
	}
}
