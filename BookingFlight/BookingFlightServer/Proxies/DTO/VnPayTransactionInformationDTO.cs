namespace BookingFlightServer.Proxies.DTO
{
	public class VnPayTransactionInformationDTO
	{
		public string OrderType { get; set; } = "";
		public decimal Amount { get; set; }
		public string OrderDescription { get; set; } = "";
		public string Name { get; set; } = "";
	}
}
