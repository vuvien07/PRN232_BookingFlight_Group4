using BookingFlightClient.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace BookingFlightClient.Controllers
{
	[Route("payment")]
	public class PaymentController : Controller
	{
		private readonly HttpClient _httpClient;

		public PaymentController(IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient();
		}
		[Route("vnPay")]
		public async Task<IActionResult> VnPayResponseIndex()
		{
			IQueryCollection query = Request.Query;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (var item in query.Keys) { 
				stringBuilder.Append(item);
				stringBuilder.Append("=");
				stringBuilder.Append(query[item]);
				stringBuilder.Append("&");
			}
			if(stringBuilder.Length > 0)
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			var response = await _httpClient.GetAsync("http://localhost:5077/api/VnPay/payment-execute?" + stringBuilder.ToString());
			if (!response.IsSuccessStatusCode)
			{
				return BadRequest(new { message = "Payment execution failed" });
			}
			var content = await response.Content.ReadAsStringAsync();
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var vnPayResponseModel = JsonSerializer
				.Deserialize<VnPayResponseModel>(content, options)
				?? new VnPayResponseModel();
			return View("~/Views/VnPayResponse.cshtml", vnPayResponseModel);
		}

	}
}
