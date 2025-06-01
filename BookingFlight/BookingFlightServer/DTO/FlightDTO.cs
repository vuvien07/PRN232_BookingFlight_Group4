using System.Text.Json;

namespace BookingFlightServer.DTO
{
	public class FlightDTO
	{
		public int FlightId { get; set; }
		public string DepartureDate { get; set; } = null!;
		public string DepartureTime { get; set; } = null!;
		public string ArrivalDate { get; set; } = null!;
		public string ArrivalTime { get; set; } = null!;
		public decimal BasePrice { get; set; }

		public string FromCode { get; set; } = null!;
		public string ToCode { get; set; } = null!;
		public string Manufacture { get; set; } = null!;
		public string PlaneCode { get; set; } = null!;
		public decimal Tax { get; set; }

		public override string ToString() => JsonSerializer.Serialize(this);
	}
}
