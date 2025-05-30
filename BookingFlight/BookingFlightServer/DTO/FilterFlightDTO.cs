namespace BookingFlightServer.DTO
{
	public class FilterFlightDTO
	{
		public int Page { get; set; }
		public int PageSize { get; set; }
		public string From { get; set; } = "";
		public string To { get; set; } = "";

		public string DepartureDate { get; set; } = "";
		public string ArrivalDate { get; set; } = "";

		public List<string> DepartureTime { get; set; } = new();
		public List<string> ArrivalTime { get; set; } = new();
		public List<string> Brands { get; set; } = new();
		public List<string> Prices { get; set; } = new();
	}
}
