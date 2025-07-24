namespace BookingFlightClient.Models.DTO
{
    public class FlightManageDTO
    {
        public int FlightId { get; set; }
        public string FlightCode { get; set; } = null!;
        public decimal Tax { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int PlaneId { get; set; }
        public string? PlaneName { get; set; }
        public int ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public int DepartureAirportId { get; set; }
        public string? DepartureAirportName { get; set; }
        public string? DepartureAirportCode { get; set; }
        public int ArrivalAirportId { get; set; }
        public string? ArrivalAirportName { get; set; }
        public string? ArrivalAirportCode { get; set; }
        public decimal BasePrice { get; set; }
        public int TotalSeats { get; set; }
        public int BookedSeats { get; set; }
        public int AvailableSeats { get; set; }
        public List<FlightServiceDTO> Services { get; set; } = new List<FlightServiceDTO>();
        public List<FlightSeatDTO> FlightSeats { get; set; } = new List<FlightSeatDTO>();
    }

    public class FlightServiceDTO
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = null!;
        public string? Detail { get; set; }
        public int ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public int? StatusId { get; set; }
        public string? StatusName { get; set; }
    }

    public class FlightSeatDTO
    {
        public int FlightId { get; set; }
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = null!;
        public bool IsSat { get; set; }
        public int? TicketId { get; set; }
        public string? TicketCode { get; set; }
        public int ClassId { get; set; }
        public string? ClassName { get; set; }
        public decimal? ClassPrice { get; set; }
        public int SeatStatusId { get; set; }
        public string? SeatStatusName { get; set; }
    }

    public class ServiceDTO
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = null!;
        public string? Detail { get; set; }
    }

    public class FlightCreateRequestDTO
    {
        public string FlightCode { get; set; } = null!;
        public decimal Tax { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int PlaneId { get; set; }
        public int DepartureAirportId { get; set; }
        public int ArrivalAirportId { get; set; }
        public int StatusId { get; set; } = 1;
        public List<int> ServiceIds { get; set; } = new List<int>();
    }

    public class FlightUpdateRequestDTO
    {
        public int FlightId { get; set; }
        public string FlightCode { get; set; } = null!;
        public decimal Tax { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int PlaneId { get; set; }
        public int DepartureAirportId { get; set; }
        public int ArrivalAirportId { get; set; }
        public int StatusId { get; set; }
        public List<int> ServiceIds { get; set; } = new List<int>();
    }

    public class AddServiceToFlightRequestDTO
    {
        public int ServiceId { get; set; }
    }

    public class FlightSeatUpdateRequestDTO
    {
        public int SeatId { get; set; }
        public bool IsSat { get; set; }
        public int? TicketId { get; set; }
    }
}
