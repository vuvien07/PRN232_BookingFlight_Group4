using Microsoft.AspNetCore.Mvc;

namespace BookingFlightClient.Controllers
{
    public class FlightController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Flight.cshtml");
        }
    }
}
