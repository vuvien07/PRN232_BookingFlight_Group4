using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
	public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
	{
		public TicketRepository(BookingFlightContext repositoryDbContext) : base(repositoryDbContext)
		{
		}

		public async Task CreateTicketAsync(Ticket ticket)
		{
			await Create(ticket);
		}

		public async Task<Ticket?> GetTicketByIdAsync(int id)
		{
			return await GetByCondition(ticket => ticket.TicketId == id,
				ticket => ticket.Include(ticket => ticket.ClassSeat)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.DepartureAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.ArrivalAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.Plane)
				.Include(ticket => ticket.Customer)
				.Include(ticket => ticket.Status)
				.Include(ticket => ticket.TicketItems).ThenInclude(ticketItem => ticketItem.Item));
		}

		public async Task<Ticket?> GetTicketByTicketNumber(string? ticketNumber)
		{
			return await GetByCondition(ticket => ticket.TicketNumber == ticketNumber,
				ticket => ticket.Include(ticket => ticket.ClassSeat).ThenInclude(cs => cs.Seats)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.DepartureAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.ArrivalAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.Plane)
			.Include(ticket => ticket.Customer)
			.Include(ticket => ticket.Status)
			.Include(ticket => ticket.TicketItems).ThenInclude(ticketItem => ticketItem.Item));
		}

		public async Task<List<Ticket>> GetAllTicketsAsync()
		{
			return await FindAll(ticket => ticket.Include(ticket => ticket.ClassSeat)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.DepartureAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.ArrivalAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.Plane)
				.Include(ticket => ticket.Customer)
				.Include(ticket => ticket.Status)
				.Include(ticket => ticket.TicketItems).ThenInclude(ticketItem => ticketItem.Item))
				.ToListAsync();
		}

		public async Task<List<Ticket>> GetTicketsByStatusAsync(int statusId)
		{
			return await FindByCondition(ticket => ticket.StatusId == statusId,
				ticket => ticket.Include(ticket => ticket.ClassSeat)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.DepartureAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.ArrivalAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.Plane)
				.Include(ticket => ticket.Customer)
				.Include(ticket => ticket.Status)
				.Include(ticket => ticket.TicketItems).ThenInclude(ticketItem => ticketItem.Item))
				.ToListAsync();
		}

		public async Task<List<Ticket>> GetTicketsByDateRangeAsync(DateTime startDate, DateTime endDate)
		{
			var startDateOnly = DateOnly.FromDateTime(startDate);
			var endDateOnly = DateOnly.FromDateTime(endDate);

			return await FindByCondition(ticket => ticket.BookingDate >= startDateOnly && ticket.BookingDate <= endDateOnly,
				ticket => ticket.Include(ticket => ticket.ClassSeat)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.DepartureAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.ArrivalAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.Plane)
				.Include(ticket => ticket.Customer)
				.Include(ticket => ticket.Status)
				.Include(ticket => ticket.TicketItems).ThenInclude(ticketItem => ticketItem.Item))
				.ToListAsync();
		}

		public async Task<(List<Ticket> tickets, int totalCount)> GetTicketsPaginatedAsync(int page, int pageSize)
		{
			var query = FindAll(ticket => ticket.Include(ticket => ticket.ClassSeat)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.DepartureAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.ArrivalAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.Plane)
				.Include(ticket => ticket.Customer)
				.Include(ticket => ticket.Status)
				.Include(ticket => ticket.TicketItems).ThenInclude(ticketItem => ticketItem.Item));

			var totalCount = await query.CountAsync();
			var tickets = await query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (tickets, totalCount);
		}

		public async Task<(List<Ticket> tickets, int totalCount)> GetTicketsByStatusPaginatedAsync(int statusId, int page, int pageSize)
		{
			var query = FindByCondition(ticket => ticket.StatusId == statusId,
				ticket => ticket.Include(ticket => ticket.ClassSeat)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.DepartureAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.ArrivalAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.Plane)
				.Include(ticket => ticket.Customer)
				.Include(ticket => ticket.Status)
				.Include(ticket => ticket.TicketItems).ThenInclude(ticketItem => ticketItem.Item));

			var totalCount = await query.CountAsync();
			var tickets = await query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (tickets, totalCount);
		}

		public async Task<(List<Ticket> tickets, int totalCount)> GetTicketsByDateRangePaginatedAsync(DateTime startDate, DateTime endDate, int page, int pageSize)
		{
			var startDateOnly = DateOnly.FromDateTime(startDate);
			var endDateOnly = DateOnly.FromDateTime(endDate);

			var query = FindByCondition(ticket => ticket.BookingDate >= startDateOnly && ticket.BookingDate <= endDateOnly,
				ticket => ticket.Include(ticket => ticket.ClassSeat)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.DepartureAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.ArrivalAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.Plane)
				.Include(ticket => ticket.Customer)
				.Include(ticket => ticket.Status)
				.Include(ticket => ticket.TicketItems).ThenInclude(ticketItem => ticketItem.Item));

			var totalCount = await query.CountAsync();
			var tickets = await query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (tickets, totalCount);
		}

		public async Task<bool> UpdateTicketAsync(Ticket ticket)
		{
			await Update(ticket);
			return true;
		}

		public async Task<bool> DeleteTicketAsync(int ticketId)
		{
			var ticket = await GetTicketByIdAsync(ticketId);
			if (ticket != null)
			{
				await Delete(ticket);
				return true;
			}
			return false;
		}

		public async Task<List<Ticket>> GetTicketsByCustomerIdAsync(int customerId)
		{
			return await FindAll(ticket => ticket.Include(ticket => ticket.ClassSeat)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.DepartureAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.ArrivalAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.Plane)
				.Include(ticket => ticket.Customer)
				.Include(ticket => ticket.Status)
				.Include(ticket => ticket.TicketItems).ThenInclude(ticketItem => ticketItem.Item))
				.Where(ticket => ticket.CustomerId == customerId)
				.OrderByDescending(ticket => ticket.BookingDate)
				.ToListAsync();
		}

		public async Task<(List<Ticket> tickets, int totalCount)> GetTicketsByCustomerIdPaginatedAsync(int customerId, int page, int pageSize)
		{
			var query = _repositoryDbContext.Tickets.AsQueryable()
				.Include(ticket => ticket.ClassSeat)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.DepartureAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.ArrivalAirport)
				.Include(ticket => ticket.Flight).ThenInclude(flight => flight.Plane)
				.Include(ticket => ticket.Customer)
				.Include(ticket => ticket.Status)
				.Include(ticket => ticket.TicketItems).ThenInclude(ticketItem => ticketItem.Item)
				.Where(ticket => ticket.CustomerId == customerId);

			var totalCount = await query.CountAsync();
			var tickets = await query.OrderByDescending(ticket => ticket.BookingDate)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (tickets, totalCount);
		}
	}
}
