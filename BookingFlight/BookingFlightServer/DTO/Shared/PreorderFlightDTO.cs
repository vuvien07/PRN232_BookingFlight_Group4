﻿using System.Text.Json;

namespace BookingFlightServer.DTO.Shared
{
	public class PreorderFlightDTO
	{
		public string Name { get; set; } = null!;
		public int Quantity { get; set; }

		public decimal TotalPrice { get; set; }
		public float Tax { get; set; }
		public decimal SeatPrice { get; set; }
		public decimal TotalFlightPrice { get; set; }

		public override string ToString() => JsonSerializer.Serialize(this);
	}
}
