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

		public TicketService(ITicketRepository ticketRepository, IMapper mapper, ILogger<TicketService> logger)
		{
			_ticketRepository = ticketRepository;
			_mapper = mapper;
			_logger = logger;
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
	}
}
