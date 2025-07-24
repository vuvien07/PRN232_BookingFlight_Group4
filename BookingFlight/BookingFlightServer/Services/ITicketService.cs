using BookingFlightServer.DTO.Shared;
using BookingFlightServer.Entities;

namespace BookingFlightServer.Services
{
	public interface ITicketService
	{
		Task<TicketDTO?> CreateTicket(Ticket ticket);
		Task<TicketDTO?> GetByTicketNumber(string? ticketNumber);
		Task<List<TicketDTO>> GetAllTickets();
		Task<PaginatedTicketResult> GetTicketsPaginated(int page, int pageSize);
		Task<List<TicketDTO>> GetTicketsByStatus(int statusId);
		Task<PaginatedTicketResult> GetTicketsByStatusPaginated(int statusId, int page, int pageSize);
		Task<List<TicketDTO>> GetTicketsByDateRange(DateTime startDate, DateTime endDate);
		Task<PaginatedTicketResult> GetTicketsByDateRangePaginated(DateTime startDate, DateTime endDate, int page, int pageSize);
		Task<TicketDTO?> GetTicketById(int ticketId);
		Task<bool> UpdateTicketStatus(int ticketId, int statusId);
		Task<bool> DeleteTicket(int ticketId);
		Task<List<TicketDTO>> GetTicketsByCustomerId(int customerId);
		Task<PaginatedTicketResult> GetTicketsByCustomerIdPaginated(int customerId, int page, int pageSize);
		Task<bool> IsCancelTicketByTicketId(int ticketId);
	}

	public class PaginatedTicketResult
	{
		public List<TicketDTO> Tickets { get; set; } = new List<TicketDTO>();
		public int TotalCount { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }
		public int TotalPages { get; set; }
		public bool HasNextPage { get; set; }
		public bool HasPreviousPage { get; set; }
	}
}
