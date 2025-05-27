using Microsoft.AspNetCore.Mvc;

namespace BookingFlightClient.Controllers
{
	public class FeedbackController : Controller
	{
		public IActionResult Index()
		{
			return View("~/Views/Feedback.cshtml");
		}
	}
}
