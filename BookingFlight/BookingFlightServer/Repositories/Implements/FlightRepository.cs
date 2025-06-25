using BookingFlightServer.DTO.Filter;
using BookingFlightServer.DTO.Query;
using BookingFlightServer.Entities;
using BookingFlightServer.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BookingFlightServer.Repositories.Implements
{
	public class FlightRepository : BaseRepository<Flight>, IFlightRepository
	{
		public FlightRepository(BookingFlightContext repositoryDbContext) : base(repositoryDbContext)
		{
		}

		public async Task<List<dynamic>> GetFlights(FilterFlightDTO filterFlightDTO)
		{
			var query = BuildFlightQuery(_repositoryDbContext);
			var result = GetByFlightCondition(query, filterFlightDTO);
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
			var query = BuildFlightQuery(_repositoryDbContext);
			var result = GetByFlightCondition(query, filterFlightDTO);
			return await result.CountAsync();
		}

		private IQueryable<FlightQueryDTO> BuildFlightQuery(BookingFlightContext _flightContext)
		{
			return from f in _flightContext.Flights
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
				   select new FlightQueryDTO
				   {
					   FlightId = g.Key.FlightId,
					   From = g.Key.From,
					   To = g.Key.To,
					   DepartureDate = g.Key.DepartureTime,
					   ArrivalDate = g.Key.ArrivalTime.ToString("yyyy-MM-dd"),
					   DepartureTime = g.Key.DepartureTime.ToString("HH:mm"),
					   ArrivalTime = g.Key.ArrivalTime.ToString("HH:mm"),
					   FromCode = g.Key.DepartureAirportCode,
					   ToCode = g.Key.ArrivalAirportCode,
					   Tax = g.Key.Tax,
					   Manufacture = g.Key.Manufacture,
					   PlaneCode = g.Key.PlaneCode,
					   BasePrice = g.Key.BasePrice,
					   Total = g.Select(x => new { x.cs.ClassId, x.fs })
		 .Where(x => x.fs != null && !x.fs.IsSat && x.ClassId != null)
		 .Select(x => x.ClassId)
		 .Distinct()
		 .Count()
				   };
		}

		private IAsyncEnumerable<FlightQueryDTO> GetByFlightCondition(IQueryable<FlightQueryDTO> query, FilterFlightDTO filterFlightDTO)
		{
			var result = FindByCondition(query,a => a.Total > 0);
			if (!string.IsNullOrEmpty(filterFlightDTO.From) && !string.IsNullOrEmpty(filterFlightDTO.To))
			{
				result = FindByCondition(result, a => a.From == int.Parse(filterFlightDTO.From) && a.To == int.Parse(filterFlightDTO.To));
			}
			if (!string.IsNullOrEmpty(filterFlightDTO.DepartureDate)
		&& DateTime.TryParseExact(filterFlightDTO.DepartureDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime filterDate))
			{
				result = FindByCondition(result, a => a.DepartureDate.Date == filterDate.Date);
			}
			if (filterFlightDTO.DepartureTime.Count() > 0)
			{
				result = FindByCondition(result, r => filterFlightDTO.DepartureTime.Any(range =>
				UtilHelper.ParseToTime(r.DepartureTime) >= UtilHelper.ParseToTime(UtilHelper.GetFromSplitString(range, "-", 0)) && UtilHelper.ParseToTime(r.DepartureTime) <= UtilHelper.ParseToTime(UtilHelper.GetFromSplitString(range, "-", 1))));
			}
			if (filterFlightDTO.ArrivalTime.Count() > 0)
			{
				result = FindByCondition(result, r => filterFlightDTO.ArrivalTime.Any(range =>
				UtilHelper.ParseToTime(r.ArrivalTime) >= UtilHelper.ParseToTime(UtilHelper.GetFromSplitString(range, "-", 0)) && UtilHelper.ParseToTime(r.ArrivalTime) <= UtilHelper.ParseToTime(UtilHelper.GetFromSplitString(range, "-", 1))));
			}
			if (filterFlightDTO.Brands.Count() > 0)
			{
				result = FindByCondition(result, r => filterFlightDTO.Brands.Any(range =>
				r.Manufacture.ToLower().Contains(range)
				));
			}
			if (filterFlightDTO.Prices.Count() > 0)
			{
				var priceRanges = filterFlightDTO.Prices
	.Select(price =>
	{
		var parts = price.Split('-');
		return new
		{
			Min = decimal.Parse(parts[0]),
			Max = decimal.Parse(parts[1])
		};
	})
	.ToList();
				var expression = BuildPriceRangeExpression(filterFlightDTO.Prices);
				result = FindByCondition(result, expression);
			}
			return result.AsAsyncEnumerable();
		}


		private Expression<Func<FlightQueryDTO, bool>> BuildPriceRangeExpression(List<string> priceRangeStrings)
		{
			var parameter = Expression.Parameter(typeof(FlightQueryDTO), "r");

			Expression? finalExpression = null;

			foreach (var priceStr in priceRangeStrings)
			{
				var parts = priceStr.Split('-');
				var min = decimal.Parse(parts[0]);
				var max = decimal.Parse(parts[1]);

				var property = Expression.Property(parameter, nameof(FlightQueryDTO.BasePrice));
				var minExpr = Expression.GreaterThanOrEqual(property, Expression.Constant(min));
				var maxExpr = Expression.LessThanOrEqual(property, Expression.Constant(max));
				var rangeExpr = Expression.AndAlso(minExpr, maxExpr);

				finalExpression = finalExpression == null
					? rangeExpr
					: Expression.OrElse(finalExpression, rangeExpr);
			}

			return finalExpression != null
				? Expression.Lambda<Func<FlightQueryDTO, bool>>(finalExpression, parameter)
				: r => true;
		}

	}
}
