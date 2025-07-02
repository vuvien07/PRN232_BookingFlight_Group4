namespace BookingFlightServer.DTO.Shared
{
	public class ClassSeatDTO
	{
		public int ClassId { get; set; }

		public string ClassName { get; set; } = null!;

		public decimal Price { get; set; }

		public string? Description { get; set; }

	}
}
