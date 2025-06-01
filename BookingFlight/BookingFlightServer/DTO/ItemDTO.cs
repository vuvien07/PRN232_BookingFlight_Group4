using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BookingFlightServer.DTO
{
	public class ItemDTO
	{
		public int ItemId { get; set; }

		public string ItemName { get; set; } = null!;

		public string Detail { get; set; } = null!;

		public int Price { get; set; }
		public int Quantity { get; set; }

		public override string ToString() => JsonSerializer.Serialize(this);
	}
}
