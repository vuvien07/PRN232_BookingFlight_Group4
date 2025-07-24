using AutoMapper;
using BookingFlightServer.DTO.Shared;
using BookingFlightServer.Entities;
using BookingFlightServer.Repositories;

namespace BookingFlightServer.Services.Implements
{
	public class TicketService : ITicketService
	{
		private readonly ITicketRepository _ticketRepository;
		private readonly IMapper _mapper;
		private readonly ILogger<TicketService> _logger;
		private readonly IFlightSeatRepository _flightSeatRepository;

		public TicketService(ITicketRepository ticketRepository, IMapper mapper, ILogger<TicketService> logger, 
			IFlightSeatRepository flightSeatRepository)
		{
			_ticketRepository = ticketRepository;
			_mapper = mapper;
			_logger = logger;
			_flightSeatRepository = flightSeatRepository;
		}

		public async Task<TicketDTO?> CreateTicket(Ticket ticket)
		{
			try
			{
				await _ticketRepository.CreateTicketAsync(ticket);
				return _mapper.Map<TicketDTO>(ticket);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating ticket: {Message}", ex.Message);
				return null;
			}
		}

		public async Task<TicketDTO?> GetByTicketNumber(string? ticketNumber)
		{
			Ticket? ticket = await _ticketRepository.GetTicketByTicketNumber(ticketNumber);
			TicketDTO ticketDTO = new();
			if (ticket != null)
			{
				ticketDTO = _mapper.Map<TicketDTO>(ticket);
				FlightSeat? flightSeat = await _flightSeatRepository.GetFlightSeatByTicketId(ticket.TicketId);
				ticketDTO.Seat = _mapper.Map<SeatDTO>(flightSeat?.Seat);
			}
			return ticketDTO;
		}

		public async Task<bool> IsCancelTicketByTicketId(int ticketId)
		{
			Ticket? ticket = await _ticketRepository.GetTicketByIdAsync(ticketId);
			if (ticket == null) return false;
			if(ticket.StatusId != 1) return false;
			if (ticket.Flight.DepartureTime - DateTime.Now < TimeSpan.FromHours(12)) return false;
			ticket.StatusId = 2;
			await _ticketRepository.UpdateTicketAsync(ticket);
			return true;
		}

		public async Task<List<TicketDTO>> GetAllTickets()
		{
			try
			{
				var tickets = await _ticketRepository.GetAllTicketsAsync();
				return _mapper.Map<List<TicketDTO>>(tickets);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting all tickets: {Message}", ex.Message);
				return new List<TicketDTO>();
			}
		}

		public async Task<List<TicketDTO>> GetTicketsByStatus(int statusId)
		{
			try
			{
				var tickets = await _ticketRepository.GetTicketsByStatusAsync(statusId);
				return _mapper.Map<List<TicketDTO>>(tickets);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting tickets by status: {Message}", ex.Message);
				return new List<TicketDTO>();
			}
		}

		public async Task<List<TicketDTO>> GetTicketsByDateRange(DateTime startDate, DateTime endDate)
		{
			try
			{
				var tickets = await _ticketRepository.GetTicketsByDateRangeAsync(startDate, endDate);
				return _mapper.Map<List<TicketDTO>>(tickets);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting tickets by date range: {Message}", ex.Message);
				return new List<TicketDTO>();
			}
		}

		public async Task<TicketDTO?> GetTicketById(int ticketId)
		{
			try
			{
				var ticket = await _ticketRepository.GetTicketByIdAsync(ticketId);
				return _mapper.Map<TicketDTO>(ticket);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting ticket by id: {Message}", ex.Message);
				return null;
			}
		}

		public async Task<PaginatedTicketResult> GetTicketsPaginated(int page, int pageSize)
		{
			try
			{
				var (tickets, totalCount) = await _ticketRepository.GetTicketsPaginatedAsync(page, pageSize);
				var ticketDTOs = _mapper.Map<List<TicketDTO>>(tickets);

				return new PaginatedTicketResult
				{
					Tickets = ticketDTOs,
					TotalCount = totalCount,
					Page = page,
					PageSize = pageSize,
					TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
					HasNextPage = page < Math.Ceiling(totalCount / (double)pageSize),
					HasPreviousPage = page > 1
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting tickets paginated: {Message}", ex.Message);
				return new PaginatedTicketResult();
			}
		}

		public async Task<PaginatedTicketResult> GetTicketsByStatusPaginated(int statusId, int page, int pageSize)
		{
			try
			{
				var (tickets, totalCount) = await _ticketRepository.GetTicketsByStatusPaginatedAsync(statusId, page, pageSize);
				var ticketDTOs = _mapper.Map<List<TicketDTO>>(tickets);

				return new PaginatedTicketResult
				{
					Tickets = ticketDTOs,
					TotalCount = totalCount,
					Page = page,
					PageSize = pageSize,
					TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
					HasNextPage = page < Math.Ceiling(totalCount / (double)pageSize),
					HasPreviousPage = page > 1
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting tickets by status paginated: {Message}", ex.Message);
				return new PaginatedTicketResult();
			}
		}

		public async Task<PaginatedTicketResult> GetTicketsByDateRangePaginated(DateTime startDate, DateTime endDate, int page, int pageSize)
		{
			try
			{
				var (tickets, totalCount) = await _ticketRepository.GetTicketsByDateRangePaginatedAsync(startDate, endDate, page, pageSize);
				var ticketDTOs = _mapper.Map<List<TicketDTO>>(tickets);

				return new PaginatedTicketResult
				{
					Tickets = ticketDTOs,
					TotalCount = totalCount,
					Page = page,
					PageSize = pageSize,
					TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
					HasNextPage = page < Math.Ceiling(totalCount / (double)pageSize),
					HasPreviousPage = page > 1
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting tickets by date range paginated: {Message}", ex.Message);
				return new PaginatedTicketResult();
			}
		}

		public async Task<bool> UpdateTicketStatus(int ticketId, int statusId)
		{
			try
			{
				var ticket = await _ticketRepository.GetTicketByIdAsync(ticketId);
				if (ticket != null)
				{
					ticket.StatusId = statusId;
					await _ticketRepository.UpdateTicketAsync(ticket);
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating ticket status: {Message}", ex.Message);
				return false;
			}
		}

		public async Task<bool> DeleteTicket(int ticketId)
		{
			try
			{
				return await _ticketRepository.DeleteTicketAsync(ticketId);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting ticket: {Message}", ex.Message);
				return false;
			}
		}

		public async Task<List<TicketDTO>> GetTicketsByCustomerId(int customerId)
		{
			try
			{
				var tickets = await _ticketRepository.GetTicketsByCustomerIdAsync(customerId);
				return _mapper.Map<List<TicketDTO>>(tickets);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting tickets by customer ID: {Message}", ex.Message);
				return new List<TicketDTO>();
			}
		}

		public async Task<PaginatedTicketResult> GetTicketsByCustomerIdPaginated(int customerId, int page, int pageSize)
		{
			try
			{
				var (tickets, totalCount) = await _ticketRepository.GetTicketsByCustomerIdPaginatedAsync(customerId, page, pageSize);
				var ticketDTOs = _mapper.Map<List<TicketDTO>>(tickets);

				return new PaginatedTicketResult
				{
					Tickets = ticketDTOs,
					TotalCount = totalCount,
					Page = page,
					PageSize = pageSize,
					TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
					HasNextPage = page * pageSize < totalCount,
					HasPreviousPage = page > 1
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting paginated tickets by customer ID: {Message}", ex.Message);
				return new PaginatedTicketResult();
			}
		}
	}
}
