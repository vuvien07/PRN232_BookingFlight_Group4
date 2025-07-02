using BookingFlightServer.Entities;

namespace BookingFlightServer.Repositories
{
	public interface ITicketItemRepository
	{
		Task CreateTicketItemAsync(TicketItem ticketItem);
	}
}
