using BookingFlightServer.DTO;
using BookingFlightServer.Entiies;
using BookingFlightServer.Utils;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
	public class FlightRepository : IFlightRepository
	{
		private readonly BookingFlightContext _flightContext;
		public FlightRepository(BookingFlightContext flightContext)
		{
			_flightContext = flightContext;
		}
		public async Task<List<dynamic>> GetFlights(FilterFlightDTO filterFlightDTO)
		{
			var query = from f in _flightContext.Flights
						join dep in _flightContext.Airports on f.DepartureAirportId equals dep.AirportId
						join arr in _flightContext.Airports on f.ArrivalAirportId equals arr.AirportId
						join p in _flightContext.Planes on f.PlaneId equals p.PlaneId
						join ap in _flightContext.AirportPrices
							on new { FromId = dep.AirportId, ToId = arr.AirportId }
							equals new { FromId = ap.AirportFromId, ToId = ap.AirportToId }
							into apGroup
						from ap in apGroup.DefaultIfEmpty()
						join fs in _flightContext.FlightSeats on f.FlightId equals fs.FlightId into fsGroup
						from fs in fsGroup.DefaultIfEmpty()
						join seat in _flightContext.Seats on new { fs.SeatId, p.PlaneId }
							equals new { seat.SeatId, seat.PlaneId }
							into seatGroup
						from seat in seatGroup.DefaultIfEmpty()
						join cs in _flightContext.ClassSeats on seat.ClassId equals cs.ClassId into csGroup
						from cs in csGroup.DefaultIfEmpty()
						group new { f, dep, arr, p, ap, fs, seat, cs } by new
						{
							f.FlightId,
							From = dep.AirportId,
							To = arr.AirportId,
							f.DepartureTime,
							f.ArrivalTime,
							DepartureAirportCode = dep.AirportCode,
							ArrivalAirportCode = arr.AirportCode,
							f.Tax,
							p.Manufacture,
							p.PlaneCode,
							ap.BasePrice,
							arr.AirportId
						} into g
						select new
						{
							g.Key.FlightId,
							g.Key.From,
							g.Key.To,
							DepartureDate = g.Key.DepartureTime.ToString("yyyy-MM-dd"),
							ArrivalDate = g.Key.ArrivalTime.ToString("yyyy-MM-dd"),
							DepartureTime = g.Key.DepartureTime.ToString("HH:mm"),
							ArrivalTime = g.Key.ArrivalTime.ToString("HH:mm"),
							FromCode = g.Key.DepartureAirportCode,
							ToCode = g.Key.ArrivalAirportCode,
							g.Key.Tax,
							g.Key.Manufacture,
							g.Key.PlaneCode,
							g.Key.BasePrice,
							g.Key.AirportId,
							Total = g.Select(x => new { x.cs.ClassId, x.fs })
		 .Where(x => x.fs != null && !x.fs.IsSat && x.ClassId != null)
		 .Select(x => x.ClassId)
		 .Distinct()
		 .Count()
						};

			var result = query.Where(x => x.Total > 0).AsAsyncEnumerable();
			if (!string.IsNullOrEmpty(filterFlightDTO.From) && !string.IsNullOrEmpty(filterFlightDTO.To))
			{
				result = result.Where(r => r.From == int.Parse(filterFlightDTO.From) && r.To == int.Parse(filterFlightDTO.To));
			}
			if (!string.IsNullOrEmpty(filterFlightDTO.DepartureDate))
			{
				result = result.Where(r => r.DepartureDate == filterFlightDTO.DepartureDate);
			}
			if (filterFlightDTO.DepartureTime.Count() > 0)
			{
				result = result.Where(r => filterFlightDTO.DepartureTime.Any(range =>
				UtilHelper.ParseToTime(r.DepartureTime) >= UtilHelper.ParseToTime(UtilHelper.GetFromSplitString(range, "-", 0)) && UtilHelper.ParseToTime(r.DepartureTime) <= UtilHelper.ParseToTime(UtilHelper.GetFromSplitString(range, "-", 1))
				));
			}
			if (filterFlightDTO.ArrivalTime.Count() > 0)
			{
				result = result.Where(r => filterFlightDTO.ArrivalTime.Any(range =>
				UtilHelper.ParseToTime(r.ArrivalTime) >= UtilHelper.ParseToTime(UtilHelper.GetFromSplitString(range, "-", 0)) && UtilHelper.ParseToTime(r.ArrivalTime) <= UtilHelper.ParseToTime(UtilHelper.GetFromSplitString(range, "-", 1))
				));
			}
			if (filterFlightDTO.Brands.Count() > 0)
			{
				result = result.Where(r => filterFlightDTO.Brands.Any(range =>
				r.PlaneCode.Contains(range)
				));
			}
			if (filterFlightDTO.Prices.Count() > 0)
			{
				result = result.Where(r => filterFlightDTO.Prices.Any(price =>
				r.BasePrice >= decimal.Parse(UtilHelper.GetFromSplitString(price, "-", 0)) && r.BasePrice <= decimal.Parse(UtilHelper.GetFromSplitString(price, "-", 1))
				));
			}
			result = result.OrderBy(f => f.FlightId);
			int totalRecords = await result.CountAsync();
			int totalPages = (int)Math.Ceiling((double)totalRecords / 5);

			// Nếu trang hợp lệ, áp dụng phân trang
			if (filterFlightDTO.Page <= totalPages)
			{
				result = result.Skip((filterFlightDTO.Page - 1) * 5).Take(5);
			}
			var results = await result.ToListAsync();
			return results.Cast<dynamic>().ToList();
		}

		public async Task<long> GetTotalFlight(FilterFlightDTO filterFlightDTO)
		{
			var query = from f in _flightContext.Flights
						join dep in _flightContext.Airports on f.DepartureAirportId equals dep.AirportId
						join arr in _flightContext.Airports on f.ArrivalAirportId equals arr.AirportId
						join p in _flightContext.Planes on f.PlaneId equals p.PlaneId
						join ap in _flightContext.AirportPrices
							on new { FromId = dep.AirportId, ToId = arr.AirportId }
							equals new { FromId = ap.AirportFromId, ToId = ap.AirportToId }
							into apGroup
						from ap in apGroup.DefaultIfEmpty()
						join fs in _flightContext.FlightSeats on f.FlightId equals fs.FlightId into fsGroup
						from fs in fsGroup.DefaultIfEmpty()
						join seat in _flightContext.Seats on new { fs.SeatId, p.PlaneId }
							equals new { seat.SeatId, seat.PlaneId }
							into seatGroup
						from seat in seatGroup.DefaultIfEmpty()
						join cs in _flightContext.ClassSeats on seat.ClassId equals cs.ClassId into csGroup
						from cs in csGroup.DefaultIfEmpty()
						group new { f, dep, arr, p, ap, fs, seat, cs } by new
						{
							f.FlightId,
							From = dep.AirportId,
							To = arr.AirportId,
							f.DepartureTime,
							f.ArrivalTime,
							DepartureAirportCode = dep.AirportCode,
							ArrivalAirportCode = arr.AirportCode,
							f.Tax,
							p.Manufacture,
							p.PlaneCode,
							ap.BasePrice,
							arr.AirportId
						} into g
						select new
						{
							g.Key.FlightId,
							g.Key.From,
							g.Key.To,
							DepartureDate = g.Key.DepartureTime.ToString("yyyy-MM-dd"),
							ArrivalDate = g.Key.ArrivalTime.ToString("yyyy-MM-dd"),
							DepartureTime = g.Key.DepartureTime.ToString("HH:mm"),
							ArrivalTime = g.Key.ArrivalTime.ToString("HH:mm"),
							FromCode = g.Key.DepartureAirportCode,
							ToCode = g.Key.ArrivalAirportCode,
							g.Key.Tax,
							g.Key.Manufacture,
							g.Key.PlaneCode,
							g.Key.BasePrice,
							g.Key.AirportId,
							Total = g.Select(x => new { x.cs.ClassId, x.fs })
		 .Where(x => x.fs != null && !x.fs.IsSat && x.ClassId != null)
		 .Select(x => x.ClassId)
		 .Distinct()
		 .Count()
						};

			var result = query.Where(x => x.Total > 0).AsAsyncEnumerable();
			if (!string.IsNullOrEmpty(filterFlightDTO.From) && !string.IsNullOrEmpty(filterFlightDTO.To))
			{
				result = result.Where(r => r.From == int.Parse(filterFlightDTO.From) && r.To == int.Parse(filterFlightDTO.To));
			}
			if (!string.IsNullOrEmpty(filterFlightDTO.DepartureDate))
			{
				result = result.Where(r => r.DepartureDate == filterFlightDTO.DepartureDate);
			}
			if (filterFlightDTO.DepartureTime.Count() > 0)
			{
				result = result.Where(r => filterFlightDTO.DepartureTime.Any(range =>
				UtilHelper.ParseToTime(r.DepartureTime) >= UtilHelper.ParseToTime(UtilHelper.GetFromSplitString(range, "-", 0)) && UtilHelper.ParseToTime(r.DepartureTime) <= UtilHelper.ParseToTime(UtilHelper.GetFromSplitString(range, "-", 1))
				));
			}
			if (filterFlightDTO.ArrivalTime.Count() > 0)
			{
				result = result.Where(r => filterFlightDTO.ArrivalTime.Any(range =>
				UtilHelper.ParseToTime(r.ArrivalTime) >= UtilHelper.ParseToTime(UtilHelper.GetFromSplitString(range, "-", 0)) && UtilHelper.ParseToTime(r.ArrivalTime) <= UtilHelper.ParseToTime(UtilHelper.GetFromSplitString(range, "-", 1))
				));
			}
			if (filterFlightDTO.Brands.Count() > 0)
			{
				result = result.Where(r => filterFlightDTO.Brands.Any(range =>
				r.PlaneCode.Contains(range)
				));
			}
			if (filterFlightDTO.Prices.Count() > 0)
			{
				result = result.Where(r => filterFlightDTO.Prices.Any(price =>
				r.BasePrice >= decimal.Parse(UtilHelper.GetFromSplitString(price, "-", 0)) && r.BasePrice <= decimal.Parse(UtilHelper.GetFromSplitString(price, "-", 1))
				));
			}
			return await result.CountAsync();
		}
	}
}
