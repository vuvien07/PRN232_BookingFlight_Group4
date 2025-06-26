using BookingFlightServer.Validations;
using System.ComponentModel.DataAnnotations;

namespace BookingFlightServer.DTO.Request
{
	public class FlightFormDTO
	{
		[Required(ErrorMessage = "Vui lòng chọn điểm đi")]
		[CompareStringWithOther("To")]
		public string From { get; set; }
		[Required(ErrorMessage = "Vui lòng chọn điểm đến")]
		[CompareStringWithOther("From")]
		public string To { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập ngày đi")]
		public string? DepartureDate { get; set; }
		[Range(0, 3, ErrorMessage =	"Số lượng hành khách trẻ em trong khoảng 0-3")]
		public int? NumChild { get; set; }
		[Range(1, 3, ErrorMessage = "Số lượng hành khách người lớn trong khoảng 1-3")]
		public int? NumAdult { get; set; }
		[Range(0, 3, ErrorMessage = "Số lượng hành khách em bé trong khoảng 0-3")]
		public int? NumInfant { get; set; }
	}
}
