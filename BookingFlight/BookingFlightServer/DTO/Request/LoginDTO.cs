using System.ComponentModel.DataAnnotations;

namespace BookingFlightServer.DTO.Request
{
	public class LoginDTO
	{
		[Required(ErrorMessage = "Username is required")]
		[RegularExpression(@"^\S*$", ErrorMessage = "Username doesn't contain space")]
		public string? Username { get; set; }
		[Required(ErrorMessage = "Password is required")]
		[RegularExpression(@"^\S*$", ErrorMessage = "Password doesn't contain space")]
		public string? Password { get; set; }
	}
}
