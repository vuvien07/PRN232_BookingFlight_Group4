using BookingFlightClient.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
            var response = await _httpClient.GetAsync(apiUrl);
			var content = await response.Content.ReadAsStringAsync();
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			ResponseTicketDTO? responseTicketDTO = null;
			if (response.IsSuccessStatusCode)
			{
				responseTicketDTO = JsonSerializer.Deserialize<ResponseTicketDTO>(content, options);
			}
            return View("~/Views/Ticket.cshtml", responseTicketDTO);
        }
    }
}
