using System.ComponentModel.DataAnnotations;

namespace BookingFlightServer.DTO.Seat
{
    // DTO hiển thị thông tin ghế cơ bản
    public class SeatDTO
    {
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty; // 1A, 1B, 2C...
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty; // Economy, Business
        public decimal ClassPrice { get; set; } // Giá ghế theo hạng
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty; // Available, Booked, Maintenance
        public int? PlaneId { get; set; }
        public string PlaneName { get; set; } = string.Empty; // Boeing 787
    }

    // DTO chi tiết ghế - cho View seat detail
    public class SeatDetailDTO
    {
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public decimal ClassPrice { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string PlaneName { get; set; } = string.Empty;
        public int TotalBookings { get; set; } // Số lần ghế được đặt
        public bool HasBookingHistory { get; set; } // Có lịch sử booking không
    }

    // DTO tạo ghế đơn lẻ - cho Create Seat
    public class CreateSeatDTO
    {
        [Required(ErrorMessage = "Seat number is required")]
        [Display(Name = "Seat Number")]
        public string SeatNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Class is required")]
        [Display(Name = "Class")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Plane is required")]
        [Display(Name = "Plane")]
        public int PlaneId { get; set; }

        [Display(Name = "Status")]
        public int StatusId { get; set; } = 1; // Mặc định Available
    }

    // DTO tạo ghế hàng loạt - cho máy bay mới
    public class BulkCreateSeatDTO
    {
        [Required(ErrorMessage = "Plane is required")]
        public int PlaneId { get; set; }

        [Range(1, 20, ErrorMessage = "Business rows must be between 1-20")]
        public int BusinessRows { get; set; } = 5; // Số hàng Business

        [Range(10, 50, ErrorMessage = "Economy rows must be between 10-50")]
        public int EconomyRows { get; set; } = 30; // Số hàng Economy

        [Required(ErrorMessage = "Seat layout is required")]
        public string SeatLayout { get; set; } = "ABCDEF"; // Layout ghế: ABCDEF
    }

    // DTO cập nhật ghế - cho Update seat
    public class UpdateSeatDTO
    {
        [Required(ErrorMessage = "Class is required")]
        [Display(Name = "Class")]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public int StatusId { get; set; }
    }

    // DTO danh sách ghế với filter
    public class SeatListDTO
    {
        public List<SeatDTO> Seats { get; set; } = new();
        public int TotalCount { get; set; }
        public int? FilterPlaneId { get; set; } // Filter theo máy bay
        public int? FilterStatusId { get; set; } // Filter theo trạng thái
    }

    // DTO cho dropdown options
    public class PlaneOptionDTO
    {
        public int PlaneId { get; set; }
        public string PlaneName { get; set; } = string.Empty;
        public string PlaneModel { get; set; } = string.Empty;
        public int CurrentSeats { get; set; } // Số ghế hiện có
    }

    public class ClassOptionDTO
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class StatusOptionDTO
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }

    // DTO data cho form
    public class SeatFormDataDTO
    {
        public List<PlaneOptionDTO> Planes { get; set; } = new();
        public List<ClassOptionDTO> Classes { get; set; } = new();
        public List<StatusOptionDTO> Statuses { get; set; } = new();
    }
}
