using System.Globalization;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using BookingFlightServer.Proxies.DTO;

namespace BookingFlightServer.Proxies.Util
{
	public class VnPayHelper
	{
		public const string VERSION = "2.1.0";
		private SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
		private SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

		public void AddRequestData(string key, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				_requestData.Add(key, value);
			}
		}

		public void AddResponseData(string key, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				_responseData.Add(key, value);
			}
		}

		public string GetResponseData(string key)
		{
			string retValue;
			if (_responseData.TryGetValue(key, out retValue))
			{
				return retValue;
			}
			else
			{
				return string.Empty;
			}
		}

		public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
		{
			var data = new StringBuilder();
			foreach (var (key, value) in _requestData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
			{

				data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
			}
			string queryString = data.ToString();

			baseUrl += "?" + queryString;
			string signData = queryString;
			if (signData.Length > 0)
			{

				signData = signData.Remove(data.Length - 1, 1);
			}
			string vnp_SecureHash = HmacSHA512(vnp_HashSecret, signData);
			baseUrl += "vnp_SecureHash=" + vnp_SecureHash;



			return baseUrl;
		}

		public bool ValidateSignature(string inputHash, string secretKey)
		{
			string rspRaw = GetResponseData();
			string myChecksum = HmacSHA512(secretKey, rspRaw);
			return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
		}
		private string GetResponseData()
		{

			StringBuilder data = new StringBuilder();
			if (_responseData.ContainsKey("vnp_SecureHashType"))
			{
				_responseData.Remove("vnp_SecureHashType");
			}
			if (_responseData.ContainsKey("vnp_SecureHash"))
			{
				_responseData.Remove("vnp_SecureHash");
			}
			foreach (KeyValuePair<string, string> kv in _responseData)
			{
				if (!string.IsNullOrEmpty(kv.Value))
				{
					data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
				}
			}
			//remove last '&'
			if (data.Length > 0)
			{
				data.Remove(data.Length - 1, 1);
			}
			return data.ToString();
		}

		public VnPayTransactionResponseDTO GetPaymentResponseModel(IQueryCollection collection, string vnp_HashSecret)
		{
			var vNPayIntergration = new VnPayHelper();
			foreach (var (key, value) in collection)
			{
				if (!string.IsNullOrEmpty(value) && key.StartsWith("vnp_"))
				{
					vNPayIntergration.AddResponseData(key, value);
				}
			}
			var orderId = Convert.ToInt64(vNPayIntergration.GetResponseData("vnp_TxnRef"));
			var vnPayTranId = Convert.ToInt64(vNPayIntergration.GetResponseData("vnp_TransactionNo"));
			var vnpResponseCode = vNPayIntergration.GetResponseData("vnp_ResponseCode");
			var vnpSecureHash =
				collection.FirstOrDefault(k => k.Key == "vnp_SecureHash").Value; //hash của dữ liệu trả về
			var orderInfo = vNPayIntergration.GetResponseData("vnp_OrderInfo");
			var checkSignature =
				vNPayIntergration.ValidateSignature(vnpSecureHash, vnp_HashSecret);
			if (!checkSignature)
				return new VnPayTransactionResponseDTO()
				{
					Success = false
				};
			return new VnPayTransactionResponseDTO()
			{
				Success = true,
				PaymentMethod = "VnPay",
				OrderDescription = orderInfo,
				OrderId = orderId.ToString(),
				PaymentId = vnPayTranId.ToString(),
				TransactionId = vnPayTranId.ToString(),
				Token = vnpSecureHash,
				VnPayResponseCode = vnpResponseCode
			};

		}
		public string GetIpAddress(HttpContext httpContext)
		{
			var remoteIpAddress = httpContext.Connection.RemoteIpAddress;

			if (remoteIpAddress != null)
			{
				var ipv4Address = Dns.GetHostEntry(remoteIpAddress).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

				return remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6 && ipv4Address != null
					? ipv4Address.ToString()
					: remoteIpAddress.ToString();
			}

			throw new InvalidOperationException("Không tìm thấy địa chỉ IP");
		}

		string HmacSHA512(string key, string inputData)
		{
			var hash = new StringBuilder();
			byte[] keyBytes = Encoding.UTF8.GetBytes(key);
			byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
			using (var hmac = new HMACSHA512(keyBytes))
			{
				byte[] hashValue = hmac.ComputeHash(inputBytes);
				foreach (var theByte in hashValue)
				{
					hash.Append(theByte.ToString("x2"));
				}
			}

			return hash.ToString();
		}
	}
	public class VnPayCompare : IComparer<string>
	{
		public int Compare(string x, string y)
		{
			if (x == y) return 0;
			if (x == null) return -1;
			if (y == null) return 1;
			var vnpCompare = CompareInfo.GetCompareInfo("en-US");
			return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
		}
	}
}
