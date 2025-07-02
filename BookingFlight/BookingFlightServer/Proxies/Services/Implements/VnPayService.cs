using BookingFlightServer.Proxies.DTO;
using BookingFlightServer.Proxies.Util;

namespace BookingFlightServer.Proxies.Services.Implements
{
	public class VnPayService : IVnPayService
	{
		private readonly IConfiguration _configuration;
		public VnPayService(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public string createPaymentUrl(VnPayTransactionInformationDTO vnPayTransaction, HttpContext httpContext)
		{
			var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["VnPay:vnp_TimeZoneId"] ?? throw new ArgumentNullException());
			var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
			var tick = DateTime.Now.Ticks.ToString();
			var pay = new VnPayHelper();

			pay.AddRequestData("vnp_Version", _configuration["VnPay:vnp_Version"] ?? throw new ArgumentNullException());
			pay.AddRequestData("vnp_Command", _configuration["VnPay:vnp_Command"] ?? throw new ArgumentNullException());
			pay.AddRequestData("vnp_TmnCode", _configuration["VnPay:vnp_TmnCode"] ?? throw new ArgumentNullException());
			pay.AddRequestData("vnp_Amount", ((int)(vnPayTransaction.Amount * 100)).ToString());
			pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
			pay.AddRequestData("vnp_CurrCode", _configuration["VnPay:vnp_CurrCode"] ?? throw new ArgumentNullException());
			pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(httpContext));
			pay.AddRequestData("vnp_Locale", _configuration["VnPay:vnp_Locale"] ?? throw new ArgumentNullException());
			pay.AddRequestData("vnp_OrderInfo", $"{vnPayTransaction.Name} {vnPayTransaction.OrderDescription} {vnPayTransaction.Amount}");
			pay.AddRequestData("vnp_OrderType", vnPayTransaction.OrderType);
			pay.AddRequestData("vnp_ReturnUrl", _configuration["VnPay:vnp_ReturnUrl"] ?? throw new ArgumentNullException());
			pay.AddRequestData("vnp_TxnRef", tick);

			var paymentUrl =
				pay.CreateRequestUrl(_configuration["VnPay:vnp_PayUrl"] ?? throw new ArgumentNullException(),
				_configuration["VnPay:vnp_HashSecret"] ?? throw new ArgumentNullException());

			return paymentUrl;
		}

		public VnPayTransactionResponseDTO paymentExecute(IQueryCollection collection)
		{
			var pay = new VnPayHelper();
			return pay.GetPaymentResponseModel(collection, _configuration["VnPay:vnp_HashSecret"] ?? throw new ArgumentNullException());
		}
	}
}
