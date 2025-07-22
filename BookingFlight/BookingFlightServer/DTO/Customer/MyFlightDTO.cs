using System.ComponentModel.DataAnnotations;

namespace BookingFlightServer.DTO.Customer
{
    public class MyFlightRequestDTO
    {
        public int? CustomerId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StatusId { get; set; } // 1: Active, 2: Cancelled, etc.
    }

    public class MyFlightResponseDTO
    {
        public int TicketId { get; set; }
        public string TicketNumber { get; set; } = null!;
        public DateOnly BookingDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Gender { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public string FullName { get; set; } = null!;
        
        // Flight Information
        public int FlightId { get; set; }
        public string FlightCode { get; set; } = null!;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string DepartureAirportName { get; set; } = null!;
        public string ArrivalAirportName { get; set; } = null!;
        public string DepartureAirportCode { get; set; } = null!;
        public string ArrivalAirportCode { get; set; } = null!;
        
        // Class Information
        public string ClassName { get; set; } = null!;
        public decimal ClassPrice { get; set; }
        
        // Seat Information
        public string? SeatNumber { get; set; }
        
        // Status Information
        public int StatusId { get; set; }
        public string StatusName { get; set; } = null!;
        
        // Contact Information
        public string? ContactFullName { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        
        // Additional Services
        public List<TicketServiceDTO>? TicketServices { get; set; }
    }

    public class TicketServiceDTO
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = null!;
        public string? Detail { get; set; }
        public int Price { get; set; }
        public int? Quantity { get; set; }
    }

    public class MyFlightCalendarDTO
    {
        public DateTime Date { get; set; }
        public List<MyFlightResponseDTO> Flights { get; set; } = new List<MyFlightResponseDTO>();
        public bool HasFlights => Flights.Any();
        public int FlightCount => Flights.Count;
    }

    public class MyFlightCalendarResponseDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<MyFlightCalendarDTO> CalendarDays { get; set; } = new List<MyFlightCalendarDTO>();
        public int TotalFlights => CalendarDays.Sum(d => d.FlightCount);
        public List<MyFlightResponseDTO> AllFlights { get; set; } = new List<MyFlightResponseDTO>();
    }

    public class CustomerFlightStatsDTO
    {
        public int TotalFlights { get; set; }
        public int UpcomingFlights { get; set; }
        public int CompletedFlights { get; set; }
        public int CancelledFlights { get; set; }
        public decimal TotalSpent { get; set; }
        public int ThisMonthFlights { get; set; }
        public int ThisYearFlights { get; set; }
    }

    public class CustomerFlightsGroupedDTO
    {
        public List<MyFlightResponseDTO> UpcomingFlights { get; set; } = new List<MyFlightResponseDTO>();
        public List<MyFlightResponseDTO> CompletedFlights { get; set; } = new List<MyFlightResponseDTO>();
        public List<MyFlightResponseDTO> CancelledFlights { get; set; } = new List<MyFlightResponseDTO>();
        public CustomerFlightStatsDTO Stats { get; set; } = new CustomerFlightStatsDTO();
    }
}
