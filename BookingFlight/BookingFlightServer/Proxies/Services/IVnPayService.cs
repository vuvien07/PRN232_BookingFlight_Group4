using BookingFlightServer.Proxies.DTO;

namespace BookingFlightServer.Proxies.Services
{
	public interface IVnPayService
	{
		string createPaymentUrl(VnPayTransactionInformationDTO vnPayTransaction, HttpContext httpContext);

		VnPayTransactionResponseDTO paymentExecute(IQueryCollection collection);
	}
}
