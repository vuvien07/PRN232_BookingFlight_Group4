using BookingFlightServer.DTO.Shared;

namespace BookingFlightServer.Services
{
	public interface ICheckoutFlightService
	{
		decimal caculateServicePrice(List<ServiceDTO> serviceDTOs);
		int caculateTotalPerson(List<PreorderFlightDTO> preorderFlightDTOs);
		Task<bool> IsSavedPassengerInformation(FlightCheckoutRequestDTO flightCheckoutRequestDTO);
	}
}
