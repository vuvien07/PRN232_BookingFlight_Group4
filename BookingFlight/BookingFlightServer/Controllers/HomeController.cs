using BookingFlightServer.DTO.Request;
using BookingFlightServer.Services;
using BookingFlightServer.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IJwtService _jwtService;

		public HomeController(IJwtService jwtService)
		{
			_jwtService = jwtService;
		}

		[HttpPost]
        public IActionResult RedirectToSearchFlight(FlightFormDTO flightFormDTO)
        {
            if (!ModelState.IsValid)
            {
				return BadRequest(UtilHelper.GetModelStateErrors(ModelState));
			}
            string flightInfoToken = _jwtService.CreateJwtDTOToken<FlightFormDTO>(flightFormDTO);
			return Ok(new { token = flightInfoToken });
        }
        [HttpGet("addSession")]
        public IActionResult AddSession()
        {
			var sessionId = Guid.NewGuid().ToString();

			var sessionTokenCookie = new CookieOptions
			{
				HttpOnly = true,
				Secure = true, // Chỉ bật nếu bạn dùng HTTPS, nếu đang dùng HTTP thì set = false
				SameSite = SameSiteMode.Strict,
				Expires = DateTime.Now.AddDays(7),
				Path = "/"
			};
			Response.Cookies.Append("X-Session-Token", sessionId, sessionTokenCookie);
			return Ok();
        }
    }
}
