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
	}
}
