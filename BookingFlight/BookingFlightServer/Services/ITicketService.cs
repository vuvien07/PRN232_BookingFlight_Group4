using BookingFlightServer.DTO.Shared;
using BookingFlightServer.Entities;

namespace BookingFlightServer.Services
{
	public interface ITicketService
	{
		Task<TicketDTO?> CreateTicket(Ticket ticket);
		Task<TicketDTO?> GetByTicketNumber(string? ticketNumber);
		Task<bool> IsCancelTicketByTicketId(int ticketId);
	}
}
