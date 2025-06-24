using Microsoft.AspNetCore.Mvc;

namespace BookingFlightClient.Controllers
{
    public class ForgotPasswordController : Controller
    {        /// <summary>
        /// Show forgot password form
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View("~/Views/Authentication/ForgotPassword.cshtml");
        }

        /// <summary>
        /// Show reset password form with token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IActionResult Reset(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Invalid reset token.";
                return RedirectToAction("Index");
            }            ViewBag.Token = token;
            return View("~/Views/Authentication/ResetPassword.cshtml");
        }
    }
}
