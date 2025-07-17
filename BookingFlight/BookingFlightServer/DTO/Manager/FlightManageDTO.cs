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
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
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

        [Required(ErrorMessage = "Customer is required")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Departure airport is required")]
        public int DepartureAirportId { get; set; }

        [Required(ErrorMessage = "Arrival airport is required")]
        [DifferentAirports(nameof(DepartureAirportId), ErrorMessage = "Arrival airport must be different from departure airport")]
        public int ArrivalAirportId { get; set; }

        public int StatusId { get; set; } = 1; // Default to active
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

        [Required(ErrorMessage = "Customer is required")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Departure airport is required")]
        public int DepartureAirportId { get; set; }

        [Required(ErrorMessage = "Arrival airport is required")]
        [DifferentAirports(nameof(DepartureAirportId), ErrorMessage = "Arrival airport must be different from departure airport")]
        public int ArrivalAirportId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public int StatusId { get; set; }
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
}
