using BookingFlightClient.Models.DTO;
using System.Text.Json;
using System.Net.Http.Headers;

namespace BookingFlightClient.Services
{
    public interface IDashboardService
    {
        Task<DashboardDataDTO> GetDashboardDataAsync();
    }

    public class DashboardService : IDashboardService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _baseUrl;

        public DashboardService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7077";
        }

        public async Task<DashboardDataDTO> GetDashboardDataAsync()
        {
            try
            {
                SetAuthorizationHeader();

                // Get dashboard statistics
                var statsResponse = await _httpClient.GetAsync($"{_baseUrl}/api/Dashboard/stats");
                var trendsResponse = await _httpClient.GetAsync($"{_baseUrl}/api/Dashboard/booking-trends");
                var routesResponse = await _httpClient.GetAsync($"{_baseUrl}/api/Dashboard/popular-routes");
                var activitiesResponse = await _httpClient.GetAsync($"{_baseUrl}/api/Dashboard/recent-activities");

                var dashboardData = new DashboardDataDTO();

                if (statsResponse.IsSuccessStatusCode)
                {
                    var statsJson = await statsResponse.Content.ReadAsStringAsync();
                    dashboardData.Stats = JsonSerializer.Deserialize<DashboardStatsDTO>(statsJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new DashboardStatsDTO();
                }

                if (trendsResponse.IsSuccessStatusCode)
                {
                    var trendsJson = await trendsResponse.Content.ReadAsStringAsync();
                    dashboardData.BookingTrends = JsonSerializer.Deserialize<List<BookingTrendDTO>>(trendsJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<BookingTrendDTO>();
                }

                if (routesResponse.IsSuccessStatusCode)
                {
                    var routesJson = await routesResponse.Content.ReadAsStringAsync();
                    dashboardData.PopularRoutes = JsonSerializer.Deserialize<List<PopularRouteDTO>>(routesJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<PopularRouteDTO>();
                }

                if (activitiesResponse.IsSuccessStatusCode)
                {
                    var activitiesJson = await activitiesResponse.Content.ReadAsStringAsync();
                    dashboardData.RecentActivities = JsonSerializer.Deserialize<List<RecentActivityDTO>>(activitiesJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<RecentActivityDTO>();
                }

                return dashboardData;
            }
            catch (Exception ex)
            {
                // Log error và trả về mock data
                Console.WriteLine($"Error getting dashboard data: {ex.Message}");
                return GetMockDashboardData();
            }
        }

        private void SetAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken") 
                       ?? _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private DashboardDataDTO GetMockDashboardData()
        {
            return new DashboardDataDTO
            {
                Stats = new DashboardStatsDTO
                {
                    TotalUsers = 1234,
                    ActiveFlights = 856,
                    BookingsToday = 2459,
                    RevenueToday = 89750,
                    UserGrowthPercent = 12.5,
                    FlightGrowthPercent = 8.2,
                    BookingGrowthPercent = -3.1,
                    RevenueGrowthPercent = 15.3
                },
                BookingTrends = new List<BookingTrendDTO>
                {
                    new() { Period = "Week 1", BookingCount = 1200, Revenue = 48000 },
                    new() { Period = "Week 2", BookingCount = 1900, Revenue = 76000 },
                    new() { Period = "Week 3", BookingCount = 1500, Revenue = 60000 },
                    new() { Period = "Week 4", BookingCount = 2100, Revenue = 84000 }
                },
                PopularRoutes = new List<PopularRouteDTO>
                {
                    new() { DepartureCode = "HAN", ArrivalCode = "SGN", DepartureName = "Hanoi", ArrivalName = "Ho Chi Minh City", BookingCount = 245, ProgressPercent = 85 },
                    new() { DepartureCode = "SGN", ArrivalCode = "HAN", DepartureName = "Ho Chi Minh City", ArrivalName = "Hanoi", BookingCount = 198, ProgressPercent = 68 },
                    new() { DepartureCode = "DAD", ArrivalCode = "SGN", DepartureName = "Da Nang", ArrivalName = "Ho Chi Minh City", BookingCount = 156, ProgressPercent = 54 }
                },
                RecentActivities = new List<RecentActivityDTO>
                {
                    new() { Type = "user", Message = "New user registered: John Smith", CreatedAt = DateTime.Now.AddMinutes(-2), Icon = "fas fa-user-plus", TimeAgo = "2 minutes ago" },
                    new() { Type = "flight", Message = "Flight VN123 scheduled for departure", CreatedAt = DateTime.Now.AddMinutes(-5), Icon = "fas fa-plane", TimeAgo = "5 minutes ago" },
                    new() { Type = "payment", Message = "Payment received: $450.00", CreatedAt = DateTime.Now.AddMinutes(-8), Icon = "fas fa-credit-card", TimeAgo = "8 minutes ago" },
                    new() { Type = "booking", Message = "Booking confirmed: BF001234", CreatedAt = DateTime.Now.AddMinutes(-12), Icon = "fas fa-ticket-alt", TimeAgo = "12 minutes ago" },
                    new() { Type = "alert", Message = "Flight AB456 delayed by 30 minutes", CreatedAt = DateTime.Now.AddMinutes(-15), Icon = "fas fa-exclamation-triangle", TimeAgo = "15 minutes ago" }
                }
            };
        }
    }
}
