namespace BookingFlightClient.Models.ViewModels
{
    public class TicketWithFeedbackStatusViewModel
    {
        public int TicketId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string PassengerName { get; set; } = string.Empty;
        public int FlightId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string DepartureAirport { get; set; } = string.Empty;
        public string ArrivalAirport { get; set; } = string.Empty;
        public DateOnly FlightDate { get; set; }
        public bool HasFeedback { get; set; }
        public int? FeedbackId { get; set; }
        public string? FeedbackTitle { get; set; }
        public int? FeedbackRate { get; set; }
        public string? FeedbackContent { get; set; }
        public DateOnly? FeedbackCreateAt { get; set; }
    }
}
