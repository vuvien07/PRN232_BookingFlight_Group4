using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
	public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
	{
		public TicketRepository(BookingFlightContext repositoryDbContext) : base(repositoryDbContext)
		{
		}

		public async Task CreateTicketAsync(Ticket ticket)
		{
			await Create(ticket);
		}

		public async Task<Ticket?> GetTicketByIdAsync(int id)
		{
			return await GetByCondition(ticket => ticket.TicketId == id);
		}

		public async Task<Ticket?> GetTicketByTicketNumber(string? ticketNumber)
		{
			return await GetByCondition(ticket => ticket.TicketNumber == ticketNumber,
				ticket => ticket.Include(ticket => ticket.ClassSeat)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.DepartureAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.ArrivalAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.Plane)
			.Include(ticket => ticket.Customer).
			Include(ticket => ticket.TicketItems).ThenInclude(ticketItem => ticketItem.Item));
		}
	}
}
