namespace BookingFlightClient.Models.ViewModels
{
    public class ComplaintViewModel
    {
        public int ComplaintId { get; set; }
        public string Description { get; set; } = null!;
        public DateTime? CreateAt { get; set; }
        public string? FileType { get; set; }
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        
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

        public string StatusCssClass => StatusId switch
        {
            1 => "badge-warning", // Pending
            2 => "badge-info",    // In Progress
            3 => "badge-success", // Resolved
            4 => "badge-danger",  // Rejected
            _ => "badge-secondary"
        };

        public string FormattedCreateDate => CreateAt?.ToString("dd/MM/yyyy HH:mm") ?? "";
    }
}
