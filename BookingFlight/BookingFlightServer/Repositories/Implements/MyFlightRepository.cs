using BookingFlightServer.Data;
using BookingFlightServer.DTO.Customer;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
    public class MyFlightRepository : BaseRepository<Ticket>, IMyFlightRepository
    {
        public MyFlightRepository(BookingFlightContext repositoryDbContext) : base(repositoryDbContext)
        {
        }

        public async Task<List<MyFlightResponseDTO>> GetCustomerFlightsAsync(int customerId, DateTime? startDate = null, DateTime? endDate = null, int? statusId = null)
        {
            var query = _repositoryDbContext.Tickets
                .Include(t => t.Flight)
                    .ThenInclude(f => f.DepartureAirport)
                .Include(t => t.Flight)
                    .ThenInclude(f => f.ArrivalAirport)
                .Include(t => t.ClassSeat)
                .Include(t => t.Status)
                .Include(t => t.FlightSeats)
                    .ThenInclude(fs => fs.Seat)
                .Include(t => t.TicketItems)
                    .ThenInclude(ti => ti.Item)
                .Where(t => t.CustomerId == customerId);

            // Apply date filters
            if (startDate.HasValue)
            {
                query = query.Where(t => t.Flight.DepartureTime >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.Flight.DepartureTime <= endDate.Value);
            }

            // Apply status filter
            if (statusId.HasValue)
            {
                query = query.Where(t => t.StatusId == statusId.Value);
            }

            var tickets = await query
                .OrderBy(t => t.Flight.DepartureTime)
                .ToListAsync();

            return tickets.Select(MapToMyFlightResponseDTO).ToList();
        }

        public async Task<MyFlightCalendarResponseDTO> GetCustomerFlightsCalendarAsync(int customerId, int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var flights = await GetCustomerFlightsAsync(customerId, startDate, endDate);

            var calendarDays = new List<MyFlightCalendarDTO>();
            var daysInMonth = DateTime.DaysInMonth(year, month);

            for (int day = 1; day <= daysInMonth; day++)
            {
                var currentDate = new DateTime(year, month, day);
                var dayFlights = flights.Where(f => f.DepartureTime.Date == currentDate.Date).ToList();

                calendarDays.Add(new MyFlightCalendarDTO
                {
                    Date = currentDate,
                    Flights = dayFlights
                });
            }

            return new MyFlightCalendarResponseDTO
            {
                Year = year,
                Month = month,
                CalendarDays = calendarDays,
                AllFlights = flights
            };
        }

        public async Task<MyFlightResponseDTO?> GetFlightDetailAsync(int ticketId, int customerId)
        {
            var ticket = await _repositoryDbContext.Tickets
                .Include(t => t.Flight)
                    .ThenInclude(f => f.DepartureAirport)
                .Include(t => t.Flight)
                    .ThenInclude(f => f.ArrivalAirport)
                .Include(t => t.ClassSeat)
                .Include(t => t.Status)
                .Include(t => t.FlightSeats)
                    .ThenInclude(fs => fs.Seat)
                .Include(t => t.TicketItems)
                    .ThenInclude(ti => ti.Item)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId && t.CustomerId == customerId);

            return ticket != null ? MapToMyFlightResponseDTO(ticket) : null;
        }

        public async Task<bool> IsTicketBelongToCustomerAsync(int ticketId, int customerId)
        {
            return await _repositoryDbContext.Tickets
                .AnyAsync(t => t.TicketId == ticketId && t.CustomerId == customerId);
        }

        public async Task<List<MyFlightResponseDTO>> GetUpcomingFlightsAsync(int customerId)
        {
            var now = DateTime.Now;
            var futureDate = now.AddDays(30);

            return await GetCustomerFlightsAsync(customerId, now, futureDate, 1); // Status 1 = Active
        }

        public async Task<List<MyFlightResponseDTO>> GetPastFlightsAsync(int customerId, int limit = 10)
        {
            var now = DateTime.Now;

            var query = _repositoryDbContext.Tickets
                .Include(t => t.Flight)
                    .ThenInclude(f => f.DepartureAirport)
                .Include(t => t.Flight)
                    .ThenInclude(f => f.ArrivalAirport)
                .Include(t => t.ClassSeat)
                .Include(t => t.Status)
                .Include(t => t.FlightSeats)
                    .ThenInclude(fs => fs.Seat)
                .Include(t => t.TicketItems)
                    .ThenInclude(ti => ti.Item)
                .Where(t => t.CustomerId == customerId && t.Flight.DepartureTime < now)
                .OrderByDescending(t => t.Flight.DepartureTime)
                .Take(limit);

            var tickets = await query.ToListAsync();
            return tickets.Select(MapToMyFlightResponseDTO).ToList();
        }

        public async Task<CustomerFlightStatsDTO> GetFlightStatsAsync(int customerId)
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfYear = new DateTime(now.Year, 1, 1);

            var allTickets = await _repositoryDbContext.Tickets
                .Include(t => t.Flight)
                .Where(t => t.CustomerId == customerId)
                .ToListAsync();

            return new CustomerFlightStatsDTO
            {
                TotalFlights = allTickets.Count,
                UpcomingFlights = allTickets.Count(t => t.Flight.DepartureTime >= now && t.StatusId == 1),
                CompletedFlights = allTickets.Count(t => t.Flight.DepartureTime < now && t.StatusId == 1),
                CancelledFlights = allTickets.Count(t => t.StatusId == 2), // Assuming 2 = Cancelled
                TotalSpent = allTickets.Sum(t => t.TotalPrice),
                ThisMonthFlights = allTickets.Count(t => t.Flight.DepartureTime >= startOfMonth && t.Flight.DepartureTime <= now),
                ThisYearFlights = allTickets.Count(t => t.Flight.DepartureTime >= startOfYear && t.Flight.DepartureTime <= now)
            };
        }

        private MyFlightResponseDTO MapToMyFlightResponseDTO(Ticket ticket)
        {
            var flightSeat = ticket.FlightSeats.FirstOrDefault();

            return new MyFlightResponseDTO
            {
                TicketId = ticket.TicketId,
                TicketNumber = ticket.TicketNumber,
                BookingDate = ticket.BookingDate,
                TotalPrice = ticket.TotalPrice,
                Gender = ticket.Gender,
                Name = ticket.Name,
                DateOfBirth = ticket.DateOfBirth,
                FullName = ticket.FullName,

                // Flight Information
                FlightId = ticket.FlightId,
                FlightCode = ticket.Flight.FlightCode,
                DepartureTime = ticket.Flight.DepartureTime,
                ArrivalTime = ticket.Flight.ArrivalTime,
                DepartureAirportName = ticket.Flight.DepartureAirport.AirportName,
                ArrivalAirportName = ticket.Flight.ArrivalAirport.AirportName,
                DepartureAirportCode = ticket.Flight.DepartureAirport.AirportCode,
                ArrivalAirportCode = ticket.Flight.ArrivalAirport.AirportCode,

                // Class Information
                ClassName = ticket.ClassSeat.ClassName,
                ClassPrice = ticket.ClassSeat.Price,

                // Seat Information
                SeatNumber = flightSeat?.Seat?.SeatNumber,

                // Status Information
                StatusId = ticket.StatusId,
                StatusName = ticket.Status.StatusName,

                // Contact Information
                ContactFullName = ticket.ContactFullName,
                ContactPhone = ticket.ContactPhone,
                ContactEmail = ticket.ContactEmail,

                // Additional Services
                TicketServices = ticket.TicketItems.Select(ti => new TicketServiceDTO
                {
                    ItemId = ti.ItemId,
                    ItemName = ti.Item.ItemName,
                    Detail = ti.Item.Detail,
                    Price = ti.Item.Price,
                    Quantity = ti.Quantity
                }).ToList()
            };
        }
    }
}
