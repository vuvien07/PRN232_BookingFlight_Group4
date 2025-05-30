namespace BookingFlightServer.DTO
{
	public class FlightCheckoutRequestDTO
	{
		public FlightDTO Flight { get; set; } = null!;
		public decimal SeatPrice { get; set; }
		public List<ServiceDTO> Services { get; set; } = null!;
		public List<PreorderFlightDTO> PreorderFlights { get; set; } = null!;
		public List<PassengerInformationFormDTO> PassengerInformationForms { get; set; } = null!;
		public string FullNameContact { get; set; } = null!;
		public string PhoneContact { get; set; } = null!;
		public string EmailContact { get; set; } = null!;
		public string AddressContact { get; set; } = null!;
		public decimal TotalAmount { get; set; }
		public int ClassSeatId { get; set; }
	}
}
