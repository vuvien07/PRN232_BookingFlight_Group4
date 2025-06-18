using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BookingFlightServer.DTO.Shared
{
	public class FlightCheckoutRequestDTO
	{
		public FlightDTO Flight { get; set; } = null!;
		public decimal SeatPrice { get; set; }
		public List<ServiceDTO> Services { get; set; } = null!;
		public List<PreorderFlightDTO> PreorderFlights { get; set; } = null!;
		public List<PassengerInformationFormDTO> PassengerInformationForms { get; set; } = null!;
		[Required(ErrorMessage ="Tên liên hệ không được để trống")]
		public string FullNameContact { get; set; } = null!;
		[Required(ErrorMessage = "Số liên hệ không được để trống")]
		[RegularExpression(@"^(0|\+84)(3|5|7|8|9)+([0-9]{8})$", ErrorMessage = "Số liên hệ không đúng định dạng")]
		public string PhoneContact { get; set; } = null!;
		[Required(ErrorMessage = "Email liên hệ không được để trống")]
		[RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email không đúng định dạng")]
		public string EmailContact { get; set; } = null!;
		[Required(ErrorMessage = "Địa chỉ liên hệ không được để trống")]
		public string AddressContact { get; set; } = null!;
		public decimal TotalAmount { get; set; }
		public int ClassSeatId { get; set; }
	}
}
