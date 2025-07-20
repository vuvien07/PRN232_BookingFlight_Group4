namespace BookingFlightServer.DTO.Manager
{
    public class TicketManageDTO
    {
        public int TicketId { get; set; }
        public string TicketNumber { get; set; } = null!;
        public string FlightCode { get; set; } = null!;
        public string DepartureAirport { get; set; } = null!;
        public string ArrivalAirport { get; set; } = null!;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string CustomerName { get; set; } = null!;
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public string PassengerName { get; set; } = null!;
        public string PassengerGender { get; set; } = null!;
        public DateTime PassengerDateOfBirth { get; set; }
        public string ClassSeat { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = null!;
        public int StatusId { get; set; }
        public string? ContactFullName { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactAddress { get; set; }
    }

    public class TicketUpdateStatusDTO
    {
        public int TicketId { get; set; }
        public int StatusId { get; set; }
    }

    public class TicketFilterDTO
    {
        public int? StatusId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SearchTerm { get; set; }
    }
}
