using Microsoft.AspNetCore.Mvc;

namespace BookingFlightClient.Controllers
{
    public class UnauthorizedController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Error/Unauthorized.cshtml");
        }
    }
}
