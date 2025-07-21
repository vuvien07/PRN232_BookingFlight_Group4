namespace BookingFlightClient.Models.Shared
{
	public class TicketItemDTO
	{
		public ItemDTO Item { get; set; } = null!;
		public int Quantity { get; set; }
	}
}
