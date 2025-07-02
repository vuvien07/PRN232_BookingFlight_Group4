using BookingFlightServer.Data;
using BookingFlightServer.Entities;

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
	}
}
