namespace BookingFlightServer.DTO
{
	public class PreorderFlightDTO
	{
		public string Name { get; set; } = null!;
		public int Quantity { get; set; }

		public decimal TotalPrice { get; set; }
		public float Tax { get; set; }
		public decimal SeatPrice { get; set; }
		public decimal TotalFlightPrice { get; set; }
	}
}
