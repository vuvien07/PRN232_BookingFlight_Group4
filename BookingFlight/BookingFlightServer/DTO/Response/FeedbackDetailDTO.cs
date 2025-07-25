namespace BookingFlightServer.DTO.Response
{
    public class FeedbackDetailDTO
    {
        public int FeedbackId { get; set; }
        public string Title { get; set; } = null!;
        public int Rate { get; set; }
        public string Content { get; set; } = null!;
        public DateOnly? CreateAt { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        
        // Ticket Information
        public int TicketId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public DateOnly BookingDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string PassengerName { get; set; } = string.Empty;
        
        // Flight Information
        public int FlightId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string DepartureAirport { get; set; } = string.Empty;
        public string ArrivalAirport { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateOnly FlightDate { get; set; }
    }
}
