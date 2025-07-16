namespace BookingFlightClient.Models.DTO
{
    public class PlaneResponse
    {
        public int PlaneId { get; set; }
        public string PlaneCode { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Manufacture { get; set; } = string.Empty;
        public int YearOfManufacture { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;
    }

    public class PlaneListRequest
    {
        public string? Search { get; set; }
        public int? StatusId { get; set; }
        public int? ManagerId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PlaneCreateRequest
    {
        public string PlaneCode { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Manufacture { get; set; } = string.Empty;
        public int YearOfManufacture { get; set; }
        public int StatusId { get; set; }
    }

    public class PlaneUpdateRequest
    {
        public int PlaneId { get; set; }
        public string PlaneCode { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Manufacture { get; set; } = string.Empty;
        public int YearOfManufacture { get; set; }
        public int StatusId { get; set; }
        public int ManagerId { get; set; }
    }

    public class StatusResponse
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }
}
