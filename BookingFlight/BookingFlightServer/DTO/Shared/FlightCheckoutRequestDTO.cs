using BookingFlightServer.Validations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BookingFlightServer.DTO.Shared
{
	public class FlightCheckoutRequestDTO
	{
		public FlightDTO Flight { get; set; } = null!;
		public decimal SeatPrice { get; set; }
		public List<ServiceDTO> Services { get; set; } = null!;
		public List<PreorderFlightDTO> PreorderFlights { get; set; } = null!;
		public List<PassengerInformationFormDTO> PassengerInformationForms { get; set; } = null!;
		[FullNameContact(["/api/CheckoutFlight/validate", "/api/CheckoutFlight/confirmService"])]
		public string FullNameContact { get; set; } = null!;
		[PhoneContact(["/api/CheckoutFlight/validate", "/api/CheckoutFlight/confirmService"])]
		public string PhoneContact { get; set; } = null!;
		[EmailContact(["/api/CheckoutFlight/validate", "/api/CheckoutFlight/confirmService"])]
		public string EmailContact { get; set; } = null!;
		[AddressContact(["/api/CheckoutFlight/validate", "/api/CheckoutFlight/confirmService"])]
		public string AddressContact { get; set; } = null!;
		public decimal TotalAmount { get; set; }
		public int ClassSeatId { get; set; }
	}
}
