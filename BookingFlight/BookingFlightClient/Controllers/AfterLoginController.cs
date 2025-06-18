using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AfterLoginController : ControllerBase
    {
        [HttpGet]
        public IActionResult SetRoleInSession([FromQuery] string role)
        {
			HttpContext.Session.SetString("sessionRole", role);
			return Ok();
        }
    }
}
