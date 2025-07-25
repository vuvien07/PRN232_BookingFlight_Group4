namespace BookingFlightClient.Models.DTO
{
    public class DashboardStatsDTO
    {
        // New properties from API
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalFlights { get; set; }
        public int TodayBookings { get; set; }
        public decimal MonthlyRevenue { get; set; }
        
        // Legacy properties for backward compatibility
        public int TotalUsers { get; set; }
        public int ActiveFlights { get; set; }
        public int BookingsToday { get; set; }
        public decimal RevenueToday { get; set; }
        public double UserGrowthPercent { get; set; }
        public double FlightGrowthPercent { get; set; }
        public double BookingGrowthPercent { get; set; }
        public double RevenueGrowthPercent { get; set; }
    }

    public class BookingTrendDTO
    {
        // New properties from API
        public string Date { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        
        // Legacy properties for backward compatibility
        public string Period { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class PopularRouteDTO
    {
        public string DepartureCode { get; set; } = string.Empty;
        public string ArrivalCode { get; set; } = string.Empty;
        public string DepartureName { get; set; } = string.Empty;
        public string ArrivalName { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public int ProgressPercent { get; set; }
    }

    public class RecentActivityDTO
    {
        // New properties from API
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        
        // Legacy properties for backward compatibility
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string TimeAgo { get; set; } = string.Empty;
    }

    public class DashboardDataDTO
    {
        public DashboardStatsDTO Stats { get; set; } = new();
        public List<BookingTrendDTO> BookingTrends { get; set; } = new();
        public List<PopularRouteDTO> PopularRoutes { get; set; } = new();
        public List<RecentActivityDTO> RecentActivities { get; set; } = new();
    }
}
