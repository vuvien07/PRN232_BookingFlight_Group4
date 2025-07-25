using Microsoft.AspNetCore.Mvc;
using BookingFlightServer.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly BookingFlightContext _context;

    public DashboardController(BookingFlightContext context)
    {
        _context = context;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var totalTickets = await _context.Tickets.CountAsync();
            var totalCustomers = await _context.Customers.CountAsync();
            var totalFlights = await _context.Flights.CountAsync();

            // Simple revenue calculation
            var totalRevenue = await _context.Tickets
                .SumAsync(t => t.TotalPrice);

            var result = new
            {
                TotalBookings = totalTickets,
                TotalRevenue = totalRevenue,
                TotalCustomers = totalCustomers,
                TotalFlights = totalFlights,
                TodayBookings = 15, // Mock data for now
                MonthlyRevenue = totalRevenue * 0.3m // Mock data
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("booking-trends")]
    public async Task<IActionResult> GetBookingTrends()
    {
        try
        {
            // Return mock data for now
            var trends = new List<object>
            {
                new { Date = "2025-07-19", BookingCount = 25 },
                new { Date = "2025-07-20", BookingCount = 32 },
                new { Date = "2025-07-21", BookingCount = 28 },
                new { Date = "2025-07-22", BookingCount = 45 },
                new { Date = "2025-07-23", BookingCount = 38 },
                new { Date = "2025-07-24", BookingCount = 52 },
                new { Date = "2025-07-25", BookingCount = 41 }
            };

            return Ok(trends);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("popular-routes")]
    public async Task<IActionResult> GetPopularRoutes()
    {
        try
        {
            // Return mock data for now
            var routes = new List<object>
            {
                new { 
                    DepartureCode = "HAN", 
                    DepartureName = "Hanoi", 
                    ArrivalCode = "SGN", 
                    ArrivalName = "Ho Chi Minh City", 
                    BookingCount = 245, 
                    ProgressPercent = 100 
                },
                new { 
                    DepartureCode = "SGN", 
                    DepartureName = "Ho Chi Minh City", 
                    ArrivalCode = "HAN", 
                    ArrivalName = "Hanoi", 
                    BookingCount = 198, 
                    ProgressPercent = 81 
                },
                new { 
                    DepartureCode = "DAD", 
                    DepartureName = "Da Nang", 
                    ArrivalCode = "SGN", 
                    ArrivalName = "Ho Chi Minh City", 
                    BookingCount = 156, 
                    ProgressPercent = 64 
                }
            };

            return Ok(routes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("recent-activities")]
    public async Task<IActionResult> GetRecentActivities()
    {
        try
        {
            // Return mock data for now
            var activities = new List<object>
            {
                new { Type = "Booking", Description = "New flight booking #12345", Time = "2 minutes ago" },
                new { Type = "Registration", Description = "New customer registration: John Doe", Time = "5 minutes ago" },
                new { Type = "Payment", Description = "Payment completed for booking #12344", Time = "8 minutes ago" },
                new { Type = "Cancellation", Description = "Flight booking cancelled", Time = "12 minutes ago" }
            };

            return Ok(activities);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
