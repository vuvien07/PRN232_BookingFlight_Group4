using BookingFlightServer.Data;
using BookingFlightServer.Entities;

namespace BookingFlightServer.Repositories.Implements
{
	public class FlightSeatRepository : BaseRepository<FlightSeat>, IFlightSeatRepository
	{
		public FlightSeatRepository(BookingFlightContext repositoryDbContext) : base(repositoryDbContext)
		{
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
