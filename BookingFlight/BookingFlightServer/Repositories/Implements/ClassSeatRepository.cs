using BookingFlightServer.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
	public class ClassSeatRepository:IClassSeatRepository
	{
		private BookingFlightContext _flightContext;

		public ClassSeatRepository(BookingFlightContext flightContext)
		{
			_flightContext = flightContext;
		}

		public async Task<long> CountAllClassSeatByFlightIdAndSeatEmpty(int flightId)
		{
			var query = (from f in _flightContext.Flights
						 join a1 in _flightContext.Airports
						on f.DepartureAirportId equals a1.AirportId
						 join a2 in _flightContext.Airports
						on f.ArrivalAirportId equals a2.AirportId
						 join p in _flightContext.Planes on f.PlaneId equals p.PlaneId
						into pGroup
						 from p in pGroup.DefaultIfEmpty()
						 join ap in _flightContext.AirportPrices on
						new { FromId = a1.AirportId, ToId = a2.AirportId } equals new { FromId = ap.AirportFromId, ToId = ap.AirportToId }
						into apGroup
						 from ap in apGroup.DefaultIfEmpty()
						 join s in _flightContext.Seats on p.PlaneId equals s.PlaneId into sGroup
						 from s in sGroup.DefaultIfEmpty()
						 join cs in _flightContext.ClassSeats on s.ClassId equals cs.ClassId into csGroup
						 from cs in csGroup.DefaultIfEmpty()
						 join fs in _flightContext.FlightSeats on new { f.FlightId, s.SeatId } equals new { fs.FlightId, fs.SeatId } into fsGroup
						 from fs in fsGroup.DefaultIfEmpty()
						 where f.FlightId == flightId && fs.IsSat == false
						 select new
						 {
							 cs.ClassId,
							 cs.ClassName,
							 cs.Price
						 }).Distinct();
			return await query.CountAsync();
		}

		public async Task<List<dynamic>> GetAllClassSeatByFlightIdAndSeatEmpty(int flightId)
		{
			var query = (from f in _flightContext.Flights
						 join a1 in _flightContext.Airports
						on f.DepartureAirportId equals a1.AirportId
						 join a2 in _flightContext.Airports
						on f.ArrivalAirportId equals a2.AirportId
						 join p in _flightContext.Planes on f.PlaneId equals p.PlaneId
						into pGroup
						 from p in pGroup.DefaultIfEmpty()
						 join ap in _flightContext.AirportPrices on
						new { FromId = a1.AirportId, ToId = a2.AirportId } equals new { FromId = ap.AirportFromId, ToId = ap.AirportToId }
						into apGroup
						 from ap in apGroup.DefaultIfEmpty()
						 join s in _flightContext.Seats on p.PlaneId equals s.PlaneId into sGroup
						 from s in sGroup.DefaultIfEmpty()
						 join cs in _flightContext.ClassSeats on s.ClassId equals cs.ClassId into csGroup
						 from cs in csGroup.DefaultIfEmpty()
						 join fs in _flightContext.FlightSeats on new { f.FlightId, s.SeatId } equals new { fs.FlightId, fs.SeatId } into fsGroup
						 from fs in fsGroup.DefaultIfEmpty()
						 where f.FlightId == flightId && fs.IsSat == false
						 select new
						 {
							 cs.ClassId,
							 cs.ClassName,
							 cs.Price
						 }).Distinct();
			return await query.Select(flight => (dynamic)flight).ToListAsync();
		}
	}
}
