using BookingFlightServer.Validations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BookingFlightServer.DTO.Shared
{
	public class PassengerInformationFormDTO
	{
		public int Id { get; set; }
		public string Gender { get; set; } = "";
		[PassengerName("Id", "Type")]
		public string Name { get; set; } = "";
		[PassengerFullName("Id", "Type")]
		public string FullName { get; set; } = "";
		[PassengerDateOfBirth("Id", "Type")]
		public string DateOfBirth { get; set; } = "";
		[PassengerCccd("Id", "Type")]
		public string Cccd { get; set; } = "";
		public string Type { get; set; } = "";
		public override string ToString() => JsonSerializer.Serialize(this);
	}
}
