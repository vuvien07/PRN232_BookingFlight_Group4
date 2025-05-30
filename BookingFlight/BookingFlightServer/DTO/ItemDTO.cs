namespace BookingFlightServer.DTO
{
	public class ItemDTO
	{
		public int ItemId { get; set; }

		public string ItemName { get; set; } = null!;

		public string Detail { get; set; } = null!;

		public int Price { get; set; }
		public int Quantity { get; set; }
	}
}
