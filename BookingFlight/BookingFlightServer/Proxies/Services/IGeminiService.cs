using BookingFlightServer.DTO.Filter;
using BookingFlightServer.DTO.Query;
using BookingFlightServer.Proxies.DTO;
using BookingFlightServer.Utils;

namespace BookingFlightServer.Proxies.Services
{
	public interface IGeminiService
	{
		Task<string> AskGeminiAsync(GeminiConversationDTO geminiConversationDT, HttpContext httpContext, FlightSearchSessionStore flightSearchSessionStore);
		Task<List<FlightQueryDTO>> GetFlightsWithGemini(FilterFlightDTO filterFlightDTO);
		Task<List<dynamic>> GetAllFlightSeatWithGeminiByFlightId(int flightId);
	}
}
