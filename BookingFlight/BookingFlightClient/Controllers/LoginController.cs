using Microsoft.AspNetCore.Mvc;

namespace BookingFlightClient.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Authentication/Login.cshtml");
        }

        public IActionResult Register()
        {
            return View("~/Views/Authentication/Register.cshtml");
        }
    }
}
