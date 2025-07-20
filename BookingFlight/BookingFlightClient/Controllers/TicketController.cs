using BookingFlightClient.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightClient.Controllers
{
    [Route("ticket")]
    public class TicketController : Controller
    {
		private readonly HttpClient _httpClient;

		public TicketController(IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient();
		}
		[Route("search")]
        public async Task<IActionResult> Index([FromQuery] string? ticketNumber)
        {
            var apiUrl = $"http://localhost:5077/api/Ticket/getByTicketNumber?ticketNumber={ticketNumber}";
			var ticket = await _httpClient.GetFromJsonAsync<ResponseTicketDTO>(apiUrl);
            return View("~/Views/Ticket.cshtml", ticket);
        }
    }
}
