using BookingFlightServer.DTO.Filter;
using BookingFlightServer.DTO.Query;
using BookingFlightServer.Proxies.DTO;
using BookingFlightServer.Proxies.Services;
using BookingFlightServer.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightServer.Proxies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeminiController : ControllerBase
    {
        private readonly IGeminiService _geminiService;
		private readonly FlightSearchSessionStore _flightSearchSessionStore;

		public GeminiController(IGeminiService geminiService, FlightSearchSessionStore flightSearchSessionStore)
		{
			_geminiService = geminiService;
			_flightSearchSessionStore = flightSearchSessionStore;
		}

		[HttpPost("ask")]
		public async Task<IActionResult> Ask([FromBody] GeminiConversationDTO geminiConversationDTO)
		{
			if (geminiConversationDTO.Prompt == null) return BadRequest("Prompt cannot be empty");
			var result = await _geminiService.AskGeminiAsync(geminiConversationDTO, HttpContext, _flightSearchSessionStore);
			return Ok(result);
		}

		[HttpPost("searchFlight")]
		public async Task<IActionResult> SearchFlight([FromBody] FilterFlightDTO filterFlightDTO)
		{
			string? token = Request.Cookies["X-Session-Token"];
			List<FlightQueryDTO> flights = await _geminiService.GetFlightsWithGemini(filterFlightDTO);
			if(token != null)
			{
				_flightSearchSessionStore.Save(token, flights);
			}
			return Ok(flights);
		}

		[HttpGet("getFlightSeats")]
		public async Task<IActionResult> GetFlightSeatsById(int flightId)
		{
			string? token = Request.Cookies["X-Session-Token"];
			var classSeats = await _geminiService.GetAllFlightSeatWithGeminiByFlightId(flightId);
			if (token != null)
			{
				_flightSearchSessionStore.Save(token, classSeats);
			}
			return Ok(classSeats);
		}
	}
}
