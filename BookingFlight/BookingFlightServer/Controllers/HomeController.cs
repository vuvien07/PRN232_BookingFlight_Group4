using BookingFlightServer.DTO;
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
    }
}
