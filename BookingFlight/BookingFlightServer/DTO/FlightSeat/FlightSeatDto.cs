namespace BookingFlightServer.DTO.FlightSeat
{
    public class UpdateSeatStatusRequest
    {
        public bool IsOccupied { get; set; }
        public int? TicketId { get; set; }
    }

    public class FlightSeatDto
    {
        public int FlightId { get; set; }
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = null!;
        public int Row { get; set; }
        public string Position { get; set; } = null!;
        public bool IsOccupied { get; set; }
        public bool IsAvailable { get; set; }
        public string SeatClass { get; set; } = null!;
        public decimal Price { get; set; }
        public int? TicketId { get; set; }
    }
}
