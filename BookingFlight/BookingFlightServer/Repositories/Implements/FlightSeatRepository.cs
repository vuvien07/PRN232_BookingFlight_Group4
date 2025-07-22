using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
	public class FlightSeatRepository : BaseRepository<FlightSeat>, IFlightSeatRepository
	{
		public FlightSeatRepository(BookingFlightContext repositoryDbContext) : base(repositoryDbContext)
		{
		}

		public async Task<FlightSeat?> GetFlightSeatByTicketId(int ticketId)
		{
			return await GetByCondition(fs => fs.TicketId == ticketId, fs => fs.Include(fs => fs.Seat));
		}

		public async Task<List<FlightSeat?>> GetFlightSeatsByFlightId(int flightId)
		{
			return await GetByCondition(fs => fs.IsSat == false && fs.FlightId == flightId)
				.ToAsyncEnumerable().ToListAsync();
		}

		public async Task UpdateFlightSeatAsync(FlightSeat flightSeat)
		{
			await Update(flightSeat);
		}
	}
}
