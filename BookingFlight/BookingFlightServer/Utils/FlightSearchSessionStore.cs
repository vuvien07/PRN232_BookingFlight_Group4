using BookingFlightServer.DTO.Query;

namespace BookingFlightServer.Utils
{
	public class FlightSearchSessionStore
	{
		private readonly Dictionary<string, List<FlightQueryDTO>> _sessionCriteria = new();
		private readonly Dictionary<string, List<dynamic>> _sessionClassSeatCriteria = new();

		public void Save(string sessionId, List<FlightQueryDTO> criteria)
		{
			_sessionCriteria[sessionId] = criteria;
		}

		public void Save(string sessionId, List<dynamic> criteria)
		{
			_sessionClassSeatCriteria[sessionId] = criteria;
		}

		public List<FlightQueryDTO>? Get(string sessionId)
		{
			_sessionCriteria.TryGetValue(sessionId, out var criteria);
			return criteria;
		}
		public List<dynamic>? GetListClassSeat(string sessionId)
		{
			_sessionClassSeatCriteria.TryGetValue(sessionId, out var criteria);
			return criteria;
		}

		public void Remove(string sessionId)
		{
			_sessionCriteria.Remove(sessionId);
		}
	}
}
