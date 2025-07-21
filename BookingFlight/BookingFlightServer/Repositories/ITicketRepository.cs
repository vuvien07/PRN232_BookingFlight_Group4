using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories
{
	public interface ITicketRepository
	{
		Task CreateTicketAsync(Ticket ticket);
		Task<Ticket?> GetTicketByIdAsync(int id);
		Task<Ticket?> GetTicketByTicketNumber(string? ticketNumber);
		Task<List<Ticket>> GetAllTicketsAsync();
		Task<(List<Ticket> tickets, int totalCount)> GetTicketsPaginatedAsync(int page, int pageSize);
		Task<List<Ticket>> GetTicketsByStatusAsync(int statusId);
		Task<(List<Ticket> tickets, int totalCount)> GetTicketsByStatusPaginatedAsync(int statusId, int page, int pageSize);
		Task<List<Ticket>> GetTicketsByDateRangeAsync(DateTime startDate, DateTime endDate);
		Task<(List<Ticket> tickets, int totalCount)> GetTicketsByDateRangePaginatedAsync(DateTime startDate, DateTime endDate, int page, int pageSize);
		Task<bool> UpdateTicketAsync(Ticket ticket);
		Task<bool> DeleteTicketAsync(int ticketId);
		Task<List<Ticket>> GetTicketsByCustomerIdAsync(int customerId);
		Task<(List<Ticket> tickets, int totalCount)> GetTicketsByCustomerIdPaginatedAsync(int customerId, int page, int pageSize);
	}
}
