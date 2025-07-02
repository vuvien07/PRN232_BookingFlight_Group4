using BookingFlightServer.Data;
using BookingFlightServer.Entities;

namespace BookingFlightServer.Repositories.Implements
{
	public class TicketItemRepository : BaseRepository<TicketItem>, ITicketItemRepository
	{
		public TicketItemRepository(BookingFlightContext repositoryDbContext) : base(repositoryDbContext)
		{
		}

		public async Task CreateTicketItemAsync(TicketItem ticketItem)
		{
			await Create(ticketItem);
		}
	}
}
