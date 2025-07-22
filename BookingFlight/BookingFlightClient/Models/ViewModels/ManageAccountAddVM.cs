using BookingFlightClient.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace BookingFlightClient.Models.ViewModels
{
    public class ManageAccountAddVM
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải có từ 3-50 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Tên đăng nhập chỉ được chứa chữ, số và dấu gạch dưới")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Mật khẩu phải chứa ít nhất 1 chữ hoa, 1 chữ thường và 1 số")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn vai trò hợp lệ")]
        public int RoleId { get; set; }

        public int StatusId { get; set; } = 1;

		public List<RoleDTO> roles { get; set; } = new List<RoleDTO>();
    }
}
