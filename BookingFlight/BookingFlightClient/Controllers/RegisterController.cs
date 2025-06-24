using Microsoft.AspNetCore.Mvc;

namespace BookingFlightClient.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Authentication/Register.cshtml");
        }

        /// <summary>
        /// Show email verification page
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IActionResult VerifyEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Invalid verification token.";
                return RedirectToAction("Index", "Login");
            }

            ViewBag.Token = token;
            return View("~/Views/Authentication/VerifyEmail.cshtml");
        }

        /// <summary>
        /// Show registration success page with instructions
        /// </summary>
        /// <returns></returns>
        public IActionResult Success()
        {
            return View("~/Views/Authentication/RegisterSuccess.cshtml");
        }
    }
}
