using Microsoft.AspNetCore.Mvc;

namespace BookingFlightClient.Controllers
{
    [Route("Support")]
    public class SupportController : Controller
    {
        [HttpGet("ai/ask")]
        public IActionResult AskAiIndex()
        {
            return View("~/Views/AISupport.cshtml");
        }
    }
}
