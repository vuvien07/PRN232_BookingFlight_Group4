using BookingFlightServer.Proxies.DTO;
using BookingFlightServer.Proxies.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightServer.Proxies.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VnPayController : ControllerBase
	{
		private readonly IVnPayService _vnPayService;

		public VnPayController(IVnPayService vnPayService)
		{
			_vnPayService = vnPayService;
		}

		[HttpPost("create-payment-url")]
		public IActionResult CreatePaymentUrl([FromBody] VnPayTransactionInformationDTO transactionInformation)
		{
			var url = _vnPayService.createPaymentUrl(transactionInformation, this.HttpContext);
			return Ok(new { url = url });
		}

		[HttpGet("payment-execute")]
		public IActionResult PaymentExecute()
		{
			var collection = Request.Query;
			var response = _vnPayService.paymentExecute(collection);
			if (response == null)
			{
				return BadRequest(new { message = "Payment execution failed" });
			}
			return Ok(response);
		}
	}
}
