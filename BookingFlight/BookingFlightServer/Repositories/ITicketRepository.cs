using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories
{
	public interface ITicketRepository
	{
		Task CreateTicketAsync(Ticket ticket);
		Task<Ticket?> GetTicketByIdAsync(int id);
	}
}
