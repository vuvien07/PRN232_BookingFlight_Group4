namespace BookingFlightServer.DTO
{
	public class PassengerInformationFormDTO
	{
		public int Id { get; set; }
		public string Gender { get; set; } = "";
		public string Name { get; set; } = "";
		public string FullName { get; set; } = "";
		public string DateOfBirth { get; set; } = "";
		public string Cccd { get; set; } = "";
		public string Type { get; set; } = "";
	}
}
