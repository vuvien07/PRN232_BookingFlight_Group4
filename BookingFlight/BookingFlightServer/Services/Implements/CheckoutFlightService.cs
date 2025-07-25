using BookingFlightServer.DTO.Shared;
using BookingFlightServer.Entities;
using BookingFlightServer.Repositories;
using BookingFlightServer.Repositories.Implements;
using BookingFlightServer.UnitOfWork;
using BookingFlightServer.Utils;

namespace BookingFlightServer.Services.Implements
{
	public class CheckoutFlightService : ICheckoutFlightService
	{
		private readonly ITicketRepository _ticketRepository;
		private readonly IClassSeatRepository _classSeatRepository;
		private readonly ITicketItemRepository _ticketItemRepository;
		private readonly IFlightSeatRepository _flightSeatRepository;
		private readonly ILogger<CheckoutFlightService> _logger;
		private readonly ITransactionDbManager _transactionDbManager;
		private readonly IEmailService _emailService;
		private readonly ICustomerRepository _customerRepository;
		private readonly IJwtService _jwtService;

		public CheckoutFlightService(ITicketRepository ticketRepository, IClassSeatRepository classSeatRepository, ITicketItemRepository ticketItemRepository,
			IFlightSeatRepository flightSeatRepository, ILogger<CheckoutFlightService> logger, ITransactionDbManager transactionDbManager,
			IEmailService emailService, ICustomerRepository customerRepository, IJwtService jwtService)
		{
			_ticketRepository = ticketRepository;
			_classSeatRepository = classSeatRepository;
			_ticketItemRepository = ticketItemRepository;
			_flightSeatRepository = flightSeatRepository;
			_logger = logger;
			_transactionDbManager = transactionDbManager;
			_emailService = emailService;
			_customerRepository = customerRepository;
			_jwtService = jwtService;
		}

		public decimal caculateServicePrice(List<ServiceDTO> serviceDTOs)
		{
			decimal total = 0;
			foreach (var service in serviceDTOs)
			{
				foreach (var item in service.Items)
				{
					total += item.Price;
				}
			}
			return total;
		}

		public int caculateTotalPerson(List<PreorderFlightDTO> preorderFlightDTOs)
		{
			return preorderFlightDTOs.Sum(p => p.Quantity);
		}

		public async Task<bool> IsSavedPassengerInformation(FlightCheckoutRequestDTO flightCheckoutRequestDTO, int customerId)
		{
			await _transactionDbManager.BeginTransactionAsync();
			try
			{
				await _emailService.TestSendMailAsync(flightCheckoutRequestDTO.EmailContact);
				bool isMailExist = await _emailService.IsEmailExistsAsync(flightCheckoutRequestDTO.EmailContact);
				if (!isMailExist) throw new Exception($@"Email {flightCheckoutRequestDTO.EmailContact} does not exist");
				List<ItemDTO> itemDTOS = flightCheckoutRequestDTO.Services.Where(s => s.Items != null).SelectMany(s => s.Items)
					.Where(i => i.Quantity > 0).ToList();
				List<string> ticketCodes = new();
				for (int i = 0; i < flightCheckoutRequestDTO.PassengerInformationForms.Count; i++)
				{
					List<FlightSeat?> flightSeats = await _flightSeatRepository.GetFlightSeatsByFlightId(flightCheckoutRequestDTO.Flight.FlightId);
					if (flightSeats == null || flightSeats.Count == 0) return false;
					var passengerInformationFormDTO = flightCheckoutRequestDTO.PassengerInformationForms[i];
					var preorderFlightDTO = flightCheckoutRequestDTO.PreorderFlights[i];
					if (preorderFlightDTO.Quantity == 0) continue;
					Ticket ticket = GenerateTicket(preorderFlightDTO, passengerInformationFormDTO, flightCheckoutRequestDTO);
					ticketCodes.Add(ticket.TicketNumber);
					if(customerId != 0) ticket.CustomerId = customerId;
					await _ticketRepository.CreateTicketAsync(ticket);
					flightSeats[0]!.IsSat = true;
					flightSeats[0]!.TicketId = ticket.TicketId;
					await _flightSeatRepository.UpdateFlightSeatAsync(flightSeats[0]!);
					foreach (var item in itemDTOS)
					{
						TicketItem ticketItem = new TicketItem();
						ticketItem.TicketId = ticket.TicketId;
						ticketItem.ItemId = item.ItemId;
						ticketItem.Quantity = item.Quantity;
						await _ticketItemRepository.CreateTicketItemAsync(ticketItem);
					}
				}
				await _emailService.SendTicketCodeByEmailAsync(ticketCodes, flightCheckoutRequestDTO);
				await _transactionDbManager.CommitTransactionAsync();
				return true;
			}
			catch (Exception ex)
			{
				await _transactionDbManager.RollbackTransactionAsync();
				_logger.LogError(ex, "Error saving passenger information: {Message}", ex.Message);
				return false;
			}
		}

		private Ticket GenerateTicket(PreorderFlightDTO preorderFlightDTO, PassengerInformationFormDTO passengerInformationFormDTO, FlightCheckoutRequestDTO flightCheckoutRequestDTO)
		{
			var ticket = new Ticket();
			ticket.FullName = passengerInformationFormDTO.FullName;
			ticket.StatusId = 1;
			ticket.TicketNumber = UtilHelper.GenerateRandomString(4) + "-" + UtilHelper.GenerateRandomString(4) + "-" + UtilHelper.GenerateRandomString(4) + "-" + UtilHelper.GenerateRandomString(4);
			ticket.FlightId = flightCheckoutRequestDTO.Flight.FlightId;
			ticket.Gender = passengerInformationFormDTO.Gender;
			ticket.DateOfBirth = DateOnly.TryParse(passengerInformationFormDTO.DateOfBirth, out var date) ? date : DateOnly.MinValue;
			ticket.BookingDate = DateOnly.FromDateTime(DateTime.Now);
			ticket.ClassSeatId = flightCheckoutRequestDTO.ClassSeatId;
			ticket.TotalPrice = CalculateTicketPrice(preorderFlightDTO, flightCheckoutRequestDTO.Flight);
			ticket.Name = preorderFlightDTO.Name;
			ticket.ContactFullName = flightCheckoutRequestDTO.FullNameContact;
			ticket.ContactPhone = flightCheckoutRequestDTO.PhoneContact;
			ticket.ContactEmail = flightCheckoutRequestDTO.EmailContact;
			ticket.ContactAddress = flightCheckoutRequestDTO.AddressContact;
			return ticket;
		}

		private decimal CalculateTicketPrice(PreorderFlightDTO preorderFlightDTO, FlightDTO flightDTO)
		{
			decimal totalPrice = 0;
			if (preorderFlightDTO.Name.Equals("Adult"))
			{
				totalPrice = flightDTO.BasePrice + ((decimal)preorderFlightDTO.Tax * flightDTO.BasePrice) + preorderFlightDTO.SeatPrice;
			}
			else if (preorderFlightDTO.Name.Equals("Child"))
			{
				totalPrice = (flightDTO.BasePrice * 0.5m) + ((decimal)preorderFlightDTO.Tax * flightDTO.BasePrice * 0.5m) + (preorderFlightDTO.SeatPrice * 0.5m);
			}
			else
			{
				totalPrice = ((decimal)preorderFlightDTO.Tax * flightDTO.BasePrice * 0.25m);
			}
			return totalPrice;
		}

		public async Task<int> GetCustomerIdByCredentials(HttpContext httpContext)
		{
			string? accessToken = httpContext.Request.Headers["X-Access-Token"];
			if (accessToken == null) return 0;
			var decodedToken = _jwtService.DecodeJwtToken(accessToken);
			string? role = decodedToken["RoleId"].ToString() ?? string.Empty;
			string? username = decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"].ToString() ?? string.Empty;
			if (role.Equals("3"))
			{
				Customer? customer = await _customerRepository.GetByUsername(username);
				if (customer == null) return 0;
				return customer.CustomerId;
			}
			return 0;
		}
	}
}
