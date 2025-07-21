namespace BookingFlightClient.Models.DTO
{
    // DTO hiển thị thông tin ghế cơ bản (Client)
    public class SeatListItemDTO
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

    // DTO response từ server (Client)
    public class SeatResponseDTO
    {
        public List<SeatListItemDTO> Seats { get; set; } = new();
        public int TotalCount { get; set; }
        public int? FilterPlaneId { get; set; }
        public int? FilterStatusId { get; set; }
    }

    // DTO chi tiết ghế (Client)
    public class SeatDetailClientDTO
    {
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public decimal ClassPrice { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string PlaneName { get; set; } = string.Empty;
        public int TotalBookings { get; set; }
        public bool HasBookingHistory { get; set; }
    }

    // DTO tạo ghế (Client)
    public class CreateSeatClientDTO
    {
        public string SeatNumber { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public int PlaneId { get; set; }
        public int StatusId { get; set; } = 1;
    }

    // DTO cập nhật ghế (Client)
    public class UpdateSeatClientDTO
    {
        public string SeatNumber { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public int StatusId { get; set; }
    }

    // DTO tạo hàng loạt (Client)
    public class BulkCreateSeatClientDTO
    {
        public int PlaneId { get; set; }
        public int ClassId { get; set; }
        public int StartRow { get; set; }
        public int EndRow { get; set; }
        public string SeatLetters { get; set; } = "ABCDEF";
        public int StatusId { get; set; } = 1;
    }

    // DTO form data (Client)
    public class SeatFormClientDataDTO
    {
        public List<PlaneClientOptionDTO> Planes { get; set; } = new();
        public List<ClassClientOptionDTO> Classes { get; set; } = new();
        public List<StatusClientOptionDTO> Statuses { get; set; } = new();
    }

    public class PlaneClientOptionDTO
    {
        public int PlaneId { get; set; }
        public string PlaneCode { get; set; } = string.Empty;
    }

    public class ClassClientOptionDTO
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class StatusClientOptionDTO
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }
}
