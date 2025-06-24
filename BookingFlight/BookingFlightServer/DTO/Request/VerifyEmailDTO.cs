using System.ComponentModel.DataAnnotations;

namespace BookingFlightServer.DTO.Request
{
    public class VerifyEmailDTO
    {
        [Required(ErrorMessage = "Verification token is required")]
        public string Token { get; set; }
    }
}
