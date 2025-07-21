namespace BookingFlightServer.Proxies.DTO
{
	public class GeminiConversationDTO
	{
		public string Prompt { get; set; } = null!;
		public bool IsProvidedNumOfPassenger { get; set; }
		public bool IsSearchFlight { get; set; }
		public bool IsSelectedFlight { get; set; }
		public bool IsSelectedClassSeat { get; set; }
		public bool IsBookedFlight { get; set; }
		public int NumAdult { get; set; }
		public int NumChild { get; set; }
		public int NumInfant { get; set; }
		public decimal FlightPrice { get; set; }
		public decimal Tax { get; set; }
		public int SelectedFlightId { get; set; }
		public int SelectedFlightSeat { get; set; }
		public decimal SelectedFlightSeatPrice { get; set; }
		public string FlightManufacture { get; set; } = null!;
		public string FlightPlaneCode { get; set; } = null!;
		public string FlightDepartureTime { get; set; } = null!;
		public string FlightArrivalTime { get; set; } = null!;
		public string FlightFromCode { get; set; } = null!;
		public string FlightToCode { get; set; } = null!;
		public string FlightDepartureDate { get; set; } = null!;


	}
}
