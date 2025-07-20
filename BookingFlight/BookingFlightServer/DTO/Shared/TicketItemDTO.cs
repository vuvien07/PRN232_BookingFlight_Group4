namespace BookingFlightServer.DTO.Shared
{
	public class TicketItemDTO
	{
		public TicketDTO Ticket { get; set; } = null!;
		public ItemDTO Item { get; set; } = null!;
		public int Quantity { get; set; }
	}
}
