namespace BookingFlightServer.Repositories
{
	public interface IClassSeatRepository
	{
		Task<List<dynamic>> GetAllClassSeatByFlightIdAndSeatEmpty(int flightId);
		Task<long> CountAllClassSeatByFlightIdAndSeatEmpty(int flightId);
	}
}
