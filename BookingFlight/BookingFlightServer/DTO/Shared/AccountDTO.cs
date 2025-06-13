namespace BookingFlightServer.DTO.Shared
{
	public class AccountDTO
	{
		public int AccountId { get; set; }

		public string Username { get; set; } = null!;

		public string? RefreshToken { get; set; }

		public RoleDTO? Role { get; set; }

		public DateTime? RefreshTokenExpiryTime { get; set; }
	}
}
