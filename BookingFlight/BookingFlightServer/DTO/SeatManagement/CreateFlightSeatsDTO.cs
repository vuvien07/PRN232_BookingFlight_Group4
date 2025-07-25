namespace BookingFlightServer.DTO.SeatManagement
{
    public class CreateFlightSeatsDTO
    {
        public int FlightId { get; set; }
        public int NumberOfRows { get; set; } = 25; // Default 25 rows
        public int SeatsPerRow { get; set; } = 6; // Default 6 seats per row (A-F)
        public int BusinessClassRows { get; set; } = 3; // First 3 rows are business
    }

    public class SeatConfigurationDTO
    {
        public int FlightId { get; set; }
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public int Row { get; set; }
        public string Position { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public bool IsOccupied { get; set; } = false;
        public string SeatClass { get; set; } = "Economy";
        public decimal Price { get; set; }
    }

    public class UpdateSeatStatusDTO
    {
        public int FlightId { get; set; }
        public int SeatId { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsOccupied { get; set; }
        public int? TicketId { get; set; }
    }

    public class FlightSeatsResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<SeatConfigurationDTO> Seats { get; set; } = new();
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public int OccupiedSeats { get; set; }
    }
}
