using System.ComponentModel.DataAnnotations;

namespace BookingFlightServer.DTO.Request
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Username is required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Username doesn't contain space")]
        [StringLength(50, ErrorMessage = "Username must be less than 50 characters")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Password doesn't contain space")]
        [StringLength(250, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 250 characters")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must match")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(50, ErrorMessage = "Full name must be less than 50 characters")]
        public string? Fullname { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(255, ErrorMessage = "Address must be less than 255 characters")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(\+84|0)[0-9]{9,10}$", ErrorMessage = "Phone number is invalid")]
        [StringLength(17, ErrorMessage = "Phone number must be less than 17 characters")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        [StringLength(50, ErrorMessage = "Email must be less than 50 characters")]
        public string? Email { get; set; }
    }
}
