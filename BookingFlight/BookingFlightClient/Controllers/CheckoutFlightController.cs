using Microsoft.AspNetCore.Mvc;

namespace BookingFlightClient.Controllers
{
    [Route("flight/checkout")]
    public class CheckoutFlightController : Controller
    {
        [Route("service")]
        public IActionResult ServiceIndex()
        {
            return View("~/Views/Service.cshtml");
        }

		[Route("typeCustomerInfo")]
		public IActionResult CheckoutIndex()
		{
			return View("~/Views/Checkout.cshtml");
		}
	}
}
