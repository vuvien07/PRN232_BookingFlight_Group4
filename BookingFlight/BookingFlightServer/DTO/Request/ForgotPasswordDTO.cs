using System.ComponentModel.DataAnnotations;

namespace BookingFlightServer.DTO.Request
{
    public class ForgotPasswordDTO
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; } = string.Empty;
    }
}
