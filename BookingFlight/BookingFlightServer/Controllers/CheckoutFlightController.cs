using BookingFlightServer.DTO.Shared;
using BookingFlightServer.Entities;
using BookingFlightServer.Services;
using BookingFlightServer.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutFlightController : ControllerBase
    {
        private readonly IFlightService _flightService;
		private readonly IJwtService _jwtService;
		private readonly ICheckoutFlightService _checkoutFlightService;

		public CheckoutFlightController(IFlightService flightService, IJwtService jwtService, ICheckoutFlightService checkoutFlightService)
		{
			_flightService = flightService;
			_jwtService = jwtService;
			_checkoutFlightService = checkoutFlightService;
		}

		[HttpPost("validate")]
		public IActionResult Validate([FromBody] FlightCheckoutRequestDTO flightCheckoutRequestDTO)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(UtilHelper.GetModelStateErrors(ModelState));
			}
			decimal totalTax = 0, totalFlightPrice = 0, totalSeatPrice = 0, totalServicePrice = 0;
			foreach (var preorderFlight in flightCheckoutRequestDTO.PreorderFlights)
			{
				if (preorderFlight.Quantity > 0 && (preorderFlight.Name.Equals("Adult") || preorderFlight.Name.Equals("Child")))
				{
					totalTax += (decimal)preorderFlight.Tax * preorderFlight.TotalFlightPrice * preorderFlight.Quantity;
					totalFlightPrice += preorderFlight.TotalFlightPrice * preorderFlight.Quantity;
					totalSeatPrice += preorderFlight.SeatPrice * preorderFlight.Quantity;
				}
				if (preorderFlight.Quantity > 0 && (preorderFlight.Name.Equals("Baby")))
				{
					totalTax += preorderFlight.TotalPrice * preorderFlight.Quantity;
				}
			}
			flightCheckoutRequestDTO.Services?.ForEach(service => {
				service.Items?.ForEach(item => {
					totalServicePrice += item.Quantity > 0 ? (service.ServiceId != 2 ? item.Price * item.Quantity : item.Price) : 0;
				});
			});
			return Ok(new
			{
				totalServicePrice,
				totalAmount = totalTax + totalFlightPrice + totalSeatPrice + totalServicePrice
			});
		}

		[HttpPost]
		public async Task<IActionResult> LoadInvoiceInfo([FromBody] FlightCheckoutRequestDTO flightCheckoutRequestDTO)
		{
			List<Service> services = await _flightService.GetServicesByFlightId(flightCheckoutRequestDTO.Flight.FlightId);
			List<ServiceDTO> serviceDTOs = new();
			foreach (Service service in services)
			{
				List<ItemDTO> itemDTOs = service.Items.Select(x => Mapper.Map<Item, ItemDTO>(x) ?? new ItemDTO()).ToList();
				ServiceDTO serviceDTO = Mapper.Map<Service, ServiceDTO>(service) ?? new ServiceDTO();
				serviceDTO.Items = itemDTOs;
				serviceDTOs.Add(serviceDTO);
			}
			flightCheckoutRequestDTO.Services = flightCheckoutRequestDTO.Services.Count == 0 ? serviceDTOs : flightCheckoutRequestDTO.Services;
			int numberOfPeople = 0;
			decimal totalTax = 0, totalFlightPrice = 0, totalSeatPrice = 0, totalServicePrice = 0;
			foreach (var preorderFlight in flightCheckoutRequestDTO.PreorderFlights)
			{
				numberOfPeople += preorderFlight.Quantity;
				if (preorderFlight.Quantity > 0 && (preorderFlight.Name.Equals("Adult") || preorderFlight.Name.Equals("Child")))
				{
					totalTax += (decimal)preorderFlight.Tax * preorderFlight.TotalFlightPrice * preorderFlight.Quantity;
					totalFlightPrice += preorderFlight.TotalFlightPrice * preorderFlight.Quantity;
					totalSeatPrice += preorderFlight.SeatPrice * preorderFlight.Quantity;
				}
				if (preorderFlight.Quantity > 0 && (preorderFlight.Name.Equals("Baby")))
				{
					totalTax += preorderFlight.TotalPrice * preorderFlight.Quantity;
				}
			}
			flightCheckoutRequestDTO.Services?.ForEach(service => {
				service.Items?.ForEach(item => {
					totalServicePrice += item.Quantity > 0 ? (service.ServiceId != 2 ? item.Price * item.Quantity : item.Price) : 0;
				});
			});


			return Ok(new
			{
				flightCheckoutRequestDTO,
				numberOfPeople,
				totalTax,
				totalFlightPrice,
				totalSeatPrice,
				totalServicePrice,
				totalPrice = totalTax + totalFlightPrice + totalSeatPrice + totalServicePrice
			});
		}


		[HttpPost("confirmService")]
		public IActionResult ConfirmServiceInfo([FromBody] FlightCheckoutRequestDTO flightCheckoutRequestDTO)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(UtilHelper.GetModelStateErrors(ModelState));
			}
			string token = _jwtService.CreateJwtDTOToken(flightCheckoutRequestDTO);
			return Ok(new { token = token });
		}

		[HttpPost("confirmPassengerForm")]
		public IActionResult ConfirmPassengerFormInfo([FromBody] FlightCheckoutRequestDTO flightCheckoutRequestDTO)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(UtilHelper.GetPassengerModelStateErrors(ModelState));
			}
			string token = _jwtService.CreateJwtDTOToken(flightCheckoutRequestDTO);
			return Ok(new { token = token });
		}
	}
}
