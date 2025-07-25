using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookingFlightClient.Services;

namespace BookingFlightClient.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public AdminController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewData["Title"] = "Admin Dashboard";
            var dashboardData = await _dashboardService.GetDashboardDataAsync();
            return View(dashboardData);
        }

        public IActionResult Users()
        {
            ViewData["Title"] = "User Management";
            return View();
        }

        public IActionResult Flights()
        {
            ViewData["Title"] = "Flight Management";
            return View();
        }

        public IActionResult Bookings()
        {
            ViewData["Title"] = "Booking Management";
            return View();
        }

        public IActionResult Reports()
        {
            ViewData["Title"] = "Reports & Analytics";
            return View();
        }

        public IActionResult Settings()
        {
            ViewData["Title"] = "System Settings";
            return View();
        }
    }
}
