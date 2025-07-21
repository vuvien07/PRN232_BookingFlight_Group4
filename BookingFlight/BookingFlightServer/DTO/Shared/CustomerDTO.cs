namespace BookingFlightServer.DTO.Shared
{
	public class CustomerDTO
	{
		public int CustomerId { get; set; }
		public string Fullname { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public int AccountId { get; set; }
	}
}
