using Microsoft.AspNetCore.Mvc;

namespace BookingFlightClient.Controllers
{
    public class FlightController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Flight.cshtml");
        }
        [HttpGet("flight/list")]
		public IActionResult GetFlightListPage()
		{
			return View("~/Views/ListFlight.cshtml");
		}
	}
}
