using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BookingFlightClient.Controllers
{
    [Authorize(Roles = "Manager")]
    public class FlightDetailsController : Controller
    {
        private readonly ILogger<FlightDetailsController> _logger;

        public FlightDetailsController(ILogger<FlightDetailsController> logger)
        {
            _logger = logger;
        }

        // GET: /FlightDetails?id={flightId}
        [HttpGet]
        public IActionResult Index(int? id)
        {
            if (!id.HasValue)
            {
                TempData["Error"] = "Flight ID is required";
                return RedirectToAction("Index", "Manager");
            }

            // Set ViewBag for sidebar
            ViewBag.UserRole = 4; // Manager role
            ViewBag.RoleName = "Manager";
            ViewBag.FlightId = id.Value;
            
            return View();
        }

        // GET: /FlightDetails/Seats?id={flightId}
        [HttpGet]
        public IActionResult Seats(int? id)
        {
            if (!id.HasValue)
            {
                TempData["Error"] = "Flight ID is required";
                return RedirectToAction("Index", "Manager");
            }

            // Set ViewBag for sidebar
            ViewBag.UserRole = 4; // Manager role
            ViewBag.RoleName = "Manager";
            ViewBag.FlightId = id.Value;
            ViewBag.Action = "seats";
            
            return View("Index");
        }

        // GET: /FlightDetails/Services?id={flightId}
        [HttpGet]
        public IActionResult Services(int? id)
        {
            if (!id.HasValue)
            {
                TempData["Error"] = "Flight ID is required";
                return RedirectToAction("Index", "Manager");
            }

            // Set ViewBag for sidebar
            ViewBag.UserRole = 4; // Manager role
            ViewBag.RoleName = "Manager";
            ViewBag.FlightId = id.Value;
            ViewBag.Action = "services";
            
            return View("Index");
        }

        // GET: /FlightDetails/Analytics?id={flightId}
        [HttpGet]
        public IActionResult Analytics(int? id)
        {
            if (!id.HasValue)
            {
                TempData["Error"] = "Flight ID is required";
                return RedirectToAction("Index", "Manager");
            }

            // Set ViewBag for sidebar
            ViewBag.UserRole = 4; // Manager role
            ViewBag.RoleName = "Manager";
            ViewBag.FlightId = id.Value;
            ViewBag.Action = "analytics";
            
            return View("Index");
        }
    }
}
