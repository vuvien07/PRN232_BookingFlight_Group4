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

		public async Task<List<FlightSeat>> GetFlightSeatsByFlightId(int flightId)
		{
			return await FindByCondition(fs => fs.IsSat == false && fs.FlightId == flightId)
				.ToListAsync();
		}

		public async Task UpdateFlightSeatAsync(FlightSeat flightSeat)
		{
			await Update(flightSeat);
		}

		public async Task<FlightSeat?> GetFlightSeatByIdAsync(int flightId, int seatId)
		{
			return await GetByCondition(fs => fs.FlightId == flightId && fs.SeatId == seatId);
		}

		public async Task<bool> UpdateFlightSeatStatusAsync(int flightId, int seatId, bool isSat, int? ticketId = null)
		{
			var flightSeat = await GetFlightSeatByIdAsync(flightId, seatId);
			if (flightSeat == null) return false;

			flightSeat.IsSat = isSat;
			flightSeat.TicketId = ticketId;
			
			await UpdateFlightSeatAsync(flightSeat);
			return true;
		}

		public async Task<bool> AssignSeatToTicketAsync(int flightId, int seatId, int ticketId)
		{
			return await UpdateFlightSeatStatusAsync(flightId, seatId, true, ticketId);
		}

		public async Task<bool> UnassignSeatAsync(int flightId, int seatId)
		{
			return await UpdateFlightSeatStatusAsync(flightId, seatId, false, null);
		}

		public async Task<List<FlightSeat>> GetAvailableFlightSeatsAsync(int flightId)
		{
			return await FindByCondition(fs => fs.FlightId == flightId && !fs.IsSat)
				.ToListAsync();
		}

		public async Task<List<FlightSeat>> GetOccupiedFlightSeatsAsync(int flightId)
		{
			return await FindByCondition(fs => fs.FlightId == flightId && fs.IsSat)
				.ToListAsync();
		}

		public async Task<int> GetAvailableSeatCountAsync(int flightId)
		{
			return await FindByCondition(fs => fs.FlightId == flightId && !fs.IsSat)
				.CountAsync();
		}

		public async Task<int> GetOccupiedSeatCountAsync(int flightId)
		{
			return await FindByCondition(fs => fs.FlightId == flightId && fs.IsSat)
				.CountAsync();
		}

		public async Task<bool> CreateFlightSeatsForFlightAsync(int flightId)
		{
			try
			{
				// Get flight information to determine plane capacity
				var flight = await _repositoryDbContext.Flights
					.Include(f => f.Plane)
					.ThenInclude(p => p.Seats)
					.FirstOrDefaultAsync(f => f.FlightId == flightId);

				if (flight?.Plane == null) return false;

				// Check if seats already exist for this flight
				var existingSeats = await FindByCondition(fs => fs.FlightId == flightId).CountAsync();
				if (existingSeats > 0) return false; // Seats already exist

				// Get capacity from plane's seats count
				var capacity = flight.Plane.Seats.Count;
				if (capacity == 0) capacity = 100; // Default capacity if no seats defined

				var seats = new List<FlightSeat>();

				// Create seats based on plane capacity
				for (int i = 1; i <= capacity; i++)
				{
					seats.Add(new FlightSeat
					{
						FlightId = flightId,
						SeatId = i,
						IsSat = false,
						TicketId = null
					});
				}

				_repositoryDbContext.Set<FlightSeat>().AddRange(seats);
				await _repositoryDbContext.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
