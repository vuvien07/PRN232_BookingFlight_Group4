using BookingFlightServer.DTO.Filter;
using BookingFlightServer.DTO.Request;
using BookingFlightServer.DTO.Shared;
using BookingFlightServer.Services;
using BookingFlightServer.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FlightController : ControllerBase
	{
		private readonly IFlightService _flightService;
		private readonly IJwtService _jwtService;
		public FlightController(IFlightService flightService, IJwtService jwtService)
		{
			_flightService = flightService;
			_jwtService = jwtService;
		}
		[HttpPost("list")]
		public async Task<IActionResult> GetFlights([FromBody] FilterFlightDTO filterFlightDTO)
		{
			var flights = await _flightService.GetFlights(filterFlightDTO);
			return Ok(new
			{
				currentPage = filterFlightDTO.Page,
				flights,
				totalPage = Math.Ceiling((double)await _flightService.GetTotalFlight(filterFlightDTO) / filterFlightDTO.PageSize)
			});
		}
		[HttpPost]
		[Route("detail")]
		public async Task<IActionResult> GetFlightDetail([FromBody] FlightDTO flightDTO)
		{
			List<dynamic> details = await _flightService.GetAllClassSeatByFlightIdAndSeatEmpty(flightDTO.FlightId);
			return Ok(new
			{
				details
			});
		}

		[HttpPost("redirect")]
		public async Task<IActionResult> RedirectToService([FromForm(Name = "flightCheckoutRequestDTO")] string flightCheckoutRequest, [FromForm(Name = "flightFormDTO")] string flightForm)
		{
			FlightFormDTO flightFormDTO = UtilHelper.DeseializeObject<FlightFormDTO>(flightForm);
			FlightCheckoutRequestDTO flightCheckoutRequestDTO = UtilHelper.DeseializeObject<FlightCheckoutRequestDTO>(flightCheckoutRequest);
			long totalSeatEmpty = await _flightService.CountAllClassSeatByFlightIdAndSeatEmpty(flightCheckoutRequestDTO.Flight.FlightId);
			if (flightFormDTO.NumAdult + flightFormDTO.NumChild > totalSeatEmpty)
			{
				throw new AppException("Không đủ chỗ ngồi để đặt. Giới hạn số lượng ghế trống: " + totalSeatEmpty);
			}
			string token = _jwtService.CreateJwtDTOToken<FlightCheckoutRequestDTO>(flightCheckoutRequestDTO);
			return Ok(new { token = token });
		}

	}
}
