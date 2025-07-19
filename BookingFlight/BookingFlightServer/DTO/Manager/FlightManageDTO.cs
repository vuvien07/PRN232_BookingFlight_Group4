using System.ComponentModel.DataAnnotations;
using BookingFlightServer.Validations;

namespace BookingFlightServer.DTO.Manager
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
        
        /// <summary>
        /// Services associated with this flight
        /// </summary>
        public List<FlightServiceDTO> Services { get; set; } = new List<FlightServiceDTO>();
        
        /// <summary>
        /// Flight seats information
        /// </summary>
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

    public class FlightListRequestDTO
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public int? StatusId { get; set; }
        public int? ManagerId { get; set; }
        public DateTime? DepartureFrom { get; set; }
        public DateTime? DepartureTo { get; set; }
        public string? DepartureAirport { get; set; }
        public string? ArrivalAirport { get; set; }
    }

    public class FlightCreateRequestDTO
    {
        [Required(ErrorMessage = "Flight code is required")]
        [StringLength(50, ErrorMessage = "Flight code cannot exceed 50 characters")]
        public string FlightCode { get; set; } = null!;

        [Required(ErrorMessage = "Tax is required")]
        [Range(0, 100, ErrorMessage = "Tax must be between 0 and 100")]
        public decimal Tax { get; set; }

        [Required(ErrorMessage = "Departure time is required")]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Arrival time is required")]
        public DateTime ArrivalTime { get; set; }

        [Required(ErrorMessage = "Plane is required")]
        public int PlaneId { get; set; }

        [Required(ErrorMessage = "Departure airport is required")]
        public int DepartureAirportId { get; set; }

        [Required(ErrorMessage = "Arrival airport is required")]
        [DifferentAirports(nameof(DepartureAirportId), ErrorMessage = "Arrival airport must be different from departure airport")]
        public int ArrivalAirportId { get; set; }

        public int StatusId { get; set; } = 1; // Default to active

        /// <summary>
        /// List of Service IDs to be associated with this flight
        /// </summary>
        public List<int> ServiceIds { get; set; } = new List<int>();
    }

    public class FlightUpdateRequestDTO
    {
        [Required(ErrorMessage = "Flight ID is required")]
        public int FlightId { get; set; }

        [Required(ErrorMessage = "Flight code is required")]
        [StringLength(50, ErrorMessage = "Flight code cannot exceed 50 characters")]
        public string FlightCode { get; set; } = null!;

        [Required(ErrorMessage = "Tax is required")]
        [Range(0, 100, ErrorMessage = "Tax must be between 0 and 100")]
        public decimal Tax { get; set; }

        [Required(ErrorMessage = "Departure time is required")]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Arrival time is required")]
        public DateTime ArrivalTime { get; set; }

        [Required(ErrorMessage = "Plane is required")]
        public int PlaneId { get; set; }

        [Required(ErrorMessage = "Departure airport is required")]
        public int DepartureAirportId { get; set; }

        [Required(ErrorMessage = "Arrival airport is required")]
        [DifferentAirports(nameof(DepartureAirportId), ErrorMessage = "Arrival airport must be different from departure airport")]
        public int ArrivalAirportId { get; set; }

        /// <summary>
        /// List of Service IDs to be associated with this flight
        /// </summary>
        public List<int> ServiceIds { get; set; } = new List<int>();
    }

    public class FlightConflictCheckRequestDTO
    {
        public int? FlightId { get; set; } // Null for create, value for update
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int PlaneId { get; set; }
        public int DepartureAirportId { get; set; }
        public int ArrivalAirportId { get; set; }
    }

    public class FlightConflictResultDTO
    {
        public bool HasConflict { get; set; }
        public List<FlightConflictDetailDTO> Conflicts { get; set; } = new();
        public string? AiAnalysis { get; set; }
        public List<string> Recommendations { get; set; } = new();
    }

    public class FlightConflictDetailDTO
    {
        public int FlightId { get; set; }
        public string FlightCode { get; set; } = null!;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string ConflictType { get; set; } = null!; // "PlaneConflict", "AirportConflict", "TimeConflict"
        public string Description { get; set; } = null!;
    }

    public class AddServiceToFlightRequestDTO
    {
        [Required(ErrorMessage = "Service ID is required")]
        public int ServiceId { get; set; }
    }

    public class FlightSeatUpdateRequestDTO
    {
        [Required(ErrorMessage = "Seat ID is required")]
        public int SeatId { get; set; }
        
        [Required(ErrorMessage = "IsSat status is required")]
        public bool IsSat { get; set; }
        
        public int? TicketId { get; set; }
    }

    public class ChangeFlightStatusRequestDTO
    {
        [Required(ErrorMessage = "Status ID is required")]
        [Range(1, 2, ErrorMessage = "Status ID must be 1 (Active) or 2 (Inactive)")]
        public int StatusId { get; set; }
    }
}
