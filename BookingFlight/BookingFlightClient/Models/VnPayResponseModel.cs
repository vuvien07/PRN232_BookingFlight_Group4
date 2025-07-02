namespace BookingFlightClient.Models
{
	public class VnPayResponseModel
	{
		public string OrderDescription { get; set; } = null!;
		public string TransactionId { get; set; } = null!;
		public string OrderId { get; set; } = null!;
		public string PaymentMethod { get; set; } = null!;
		public string PaymentId { get; set; } = null!;
		public bool Success { get; set; }
		public string Token { get; set; } = null!;
		public string VnPayResponseCode { get; set; } = null!;
	}
}
