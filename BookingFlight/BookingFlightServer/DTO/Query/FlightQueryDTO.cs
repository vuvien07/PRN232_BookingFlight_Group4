namespace BookingFlightServer.DTO.Query
{
	public class FlightQueryDTO
	{
		public int FlightId { get; set; }
		public int From { get; set; }
		public int To { get; set; }
		public string DepartureDate { get; set; } = null!;
		public string ArrivalDate { get; set; } = null!;
		public string DepartureTime { get; set; } = null!;
		public string ArrivalTime { get; set; } = null!;
		public string FromCode { get; set; } = null!;
		public string ToCode { get; set; } = null!;
		public decimal Tax { get; set; }
		public	string Manufacture { get; set; } = null!;
		public string PlaneCode { get; set; } = null!;	
		public decimal BasePrice { get; set; }
		public int AirportId { get; set; }
		public int Total { get; set; }
	}
}
