namespace BookingFlightServer.DTO.Manager
{
    public class DiscountDTO
    {
        public int DiscountId { get; set; }
        public string DiscountCode { get; set; } = null!;
        public decimal DiscountPercent { get; set; }
        public string DiscountTitle { get; set; } = null!;
        public string? DiscountInfor { get; set; }
        public byte? Status { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
    }

    public class DiscountListRequestDTO
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public byte? Status { get; set; }
        public int? CustomerId { get; set; }
    }

    public class CreateDiscountDTO
    {
        public string DiscountCode { get; set; } = null!;
        public decimal DiscountPercent { get; set; }
        public string DiscountTitle { get; set; } = null!;
        public string? DiscountInfor { get; set; }
        public byte Status { get; set; } = 1;
        public int CustomerId { get; set; }
    }

    public class UpdateDiscountDTO
    {
        public string DiscountCode { get; set; } = null!;
        public decimal DiscountPercent { get; set; }
        public string DiscountTitle { get; set; } = null!;
        public string? DiscountInfor { get; set; }
        public byte Status { get; set; }
        public int CustomerId { get; set; }
    }

    public class DiscountCreateRequestDTO
    {
        public string DiscountCode { get; set; } = null!;
        public decimal DiscountPercent { get; set; }
        public string DiscountTitle { get; set; } = null!;
        public string? DiscountInfor { get; set; }
        public byte Status { get; set; } = 1;
        public int CustomerId { get; set; }
    }

    public class DiscountUpdateRequestDTO
    {
        public int DiscountId { get; set; }
        public string DiscountCode { get; set; } = null!;
        public decimal DiscountPercent { get; set; }
        public string DiscountTitle { get; set; } = null!;
        public string? DiscountInfor { get; set; }
        public byte Status { get; set; }
        public int CustomerId { get; set; }
    }
}
