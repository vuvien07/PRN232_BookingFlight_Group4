namespace BookingFlightServer.DTO
{
	public class ServiceDTO
	{
		public int ServiceId { get; set; }

		public string ServiceName { get; set; } = null!;

		public string Detail { get; set; } = null!;
		public List<ItemDTO> Items { get; set; } = null!;
	}
}
