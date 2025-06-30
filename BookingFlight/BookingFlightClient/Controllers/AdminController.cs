using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BookingFlightClient.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Admin Dashboard";
            return View();
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
