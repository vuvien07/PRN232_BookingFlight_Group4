namespace BookingFlightServer.DTO.Response
{
    public class ComplaintResponseDto
    {
        public int ComplaintId { get; set; }
        public string Description { get; set; } = null!;
        public DateTime? CreateAt { get; set; }
        public string? FileType { get; set; }
        public string? FileUrl { get; set; }
        // public string? FileName { get; set; } // Column doesn't exist in DB
        
        // Status Information
        public int StatusId { get; set; }
        public string StatusName { get; set; } = null!;
        
        // Customer Information
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        
        // Supporter Information
        public int SupporterId { get; set; }
        public string? SupporterName { get; set; }
        public string? SupporterEmail { get; set; }
    }
}
