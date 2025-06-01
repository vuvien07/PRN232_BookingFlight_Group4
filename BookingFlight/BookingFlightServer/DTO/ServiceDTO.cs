using BookingFlightServer.Validations;
using System.Text.Json;

namespace BookingFlightServer.DTO
{
	public class ServiceDTO
	{
		public int ServiceId { get; set; }

		public string ServiceName { get; set; } = null!;

		public string Detail { get; set; } = null!;
		[ValidQuantity("ServiceId")]
		public List<ItemDTO> Items { get; set; } = null!;

		public override string ToString() => JsonSerializer.Serialize(this);
	}
}
