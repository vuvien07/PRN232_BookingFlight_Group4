namespace BookingFlightServer.DTO.Shared
{
    public class AirportDTO
    {
        public int AirportId { get; set; }
        public string AirportCode { get; set; } = string.Empty;
        public string AirportName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class PlaneDTO
    {
        public int PlaneId { get; set; }
        public string PlaneCode { get; set; } = string.Empty;
        public string PlaneModel { get; set; } = string.Empty;
        public string Manufacture { get; set; } = string.Empty;
    }

    public class DetailedFlightDTO
    {
        public int FlightId { get; set; }
        public string FlightCode { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal BasePrice { get; set; }
        public decimal Tax { get; set; }
        public int StatusId { get; set; }
        
        public AirportDTO DepartureAirport { get; set; } = new();
        public AirportDTO ArrivalAirport { get; set; } = new();
        public PlaneDTO Plane { get; set; } = new();
    }
}
