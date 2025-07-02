using BookingFlightServer.Entities;

namespace BookingFlightServer.Repositories
{
	public interface IClassSeatRepository
	{
		Task<ClassSeat?> GetClassSeatByIdAsync(int id);
		Task UpdateClassSeatAsync(ClassSeat classSeat);
		Task<List<dynamic>> GetAllClassSeatByFlightIdAndSeatEmpty(int flightId);
		Task<long> CountAllClassSeatByFlightIdAndSeatEmpty(int flightId);
	}
}
