using BookingFlightServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

		public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }
        [Route("getByTicketNumber")]
		[HttpGet]
        public async Task<IActionResult> GetByTicketNumber([FromQuery] string? ticketNumber)
        {
            return Ok(await _ticketService.GetByTicketNumber(ticketNumber));
        }
    }
}
