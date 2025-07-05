using BookingFlightServer.DTO.Shared;

namespace BookingFlightServer.DTO.Manager
{
    public class ServiceDTO
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = null!;
        public string? Detail { get; set; }
        public int ManagerId { get; set; }
        public int? StatusId { get; set; }
        public StatusDTO? Status { get; set; }
        public string? ManagerName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int FlightCount { get; set; }
        public int ItemCount { get; set; }
    }

    public class ServiceListRequestDTO
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public int? StatusId { get; set; }
        public int? ManagerId { get; set; }
    }

    public class ServiceCreateRequestDTO
    {
        public string ServiceName { get; set; } = null!;
        public string? Detail { get; set; }
        public int ManagerId { get; set; }
        public int? StatusId { get; set; }
    }

    public class ServiceUpdateRequestDTO
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = null!;
        public string? Detail { get; set; }
        public int? StatusId { get; set; }
    }
}
