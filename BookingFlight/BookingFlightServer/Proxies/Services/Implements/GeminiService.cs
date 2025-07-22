using BookingFlightServer.DTO.Filter;
using BookingFlightServer.DTO.Query;
using BookingFlightServer.Entities;
using BookingFlightServer.Proxies.DTO;
using BookingFlightServer.Services;
using BookingFlightServer.Utils;
using System.Net.Http;
using System.Text.Json;

namespace BookingFlightServer.Proxies.Services.Implements
{
	public class GeminiService : IGeminiService
	{
		private readonly HttpClient _httpClient;
		private readonly IFlightService _flightService;
		private readonly IConfiguration _configuration;
		public GeminiService(IHttpClientFactory httpClientFactory, IFlightService flightService, IConfiguration configuration)
		{
			_httpClient = httpClientFactory.CreateClient();
			_flightService = flightService;
			_configuration = configuration;
		}

		public async Task<string> AskGeminiAsync(GeminiConversationDTO geminiConversationDTO, HttpContext httpContext, FlightSearchSessionStore flightSearchSessionStore)
		{
			var apiKey = _configuration["GeminiAI:ApiKey"];
			var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";
			var internalContext = "";
			string? sessionId = httpContext.Request.Headers["X-Session-Token"];
			if (sessionId != null)
			{
				List<FlightQueryDTO> flightList = flightSearchSessionStore.Get(sessionId) ?? new List<FlightQueryDTO>();
				internalContext += $@"
                    Thông tin hệ thống:
                Website đặt máy bay tên là Booking Flight.
	            
                Các chuyến bay trong hệ thống:
				";
				foreach(var flight in flightList)
				{
					internalContext += $@"
                - Id chuyến bay: {flight.FlightId}
                - Mã chuyến bay: {flight.FlightCode}
				  - Từ: {flight.From} ({flight.FromCode}) đến {flight.To} ({flight.ToCode})
				  - Thời gian: {flight.DepartureTime} - {flight.ArrivalTime}
				  - Thông tin máy bay: 
					+ Mã máy bay: {flight.PlaneCode}
					+ Nhà sản xuất: {flight.Manufacture}
				  - Số lượng hạng ghế: {flight.Total}
				  - Phí vận chuyển: {flight.Tax}
				  - Giá cả: {flight.BasePrice}
					";
				}
				internalContext += $@"
				Dữ liệu này sẽ dùng để trả lời câu hỏi liên quan đến chọn chuyến bay của người dùng.
				";
				List<dynamic> flightSeatList = flightSearchSessionStore.GetListClassSeat(sessionId) ?? new List<dynamic>();
				internalContext += $@"
                    Thông tin hệ thống:
                Website đặt máy bay tên là Booking Flight.
	            
                Danh sách các hạng ghế theo chuyến bay khách hàng đã chọn:
				";
				foreach (var flightSeat in flightSeatList)
				{
					internalContext += $@"
				- Id hạng ghế: {flightSeat.ClassId}
				- Tên hạng ghế: {flightSeat.ClassName}
	            - Giá cả hạng ghế: {flightSeat.Price}
					";						
				}
				internalContext += $@"
				Dữ liệu này sẽ dùng để trả lời câu hỏi liên quan đến chọn hạng ghế trong chuyến bay của người dùng.
				";
			}
			if (!geminiConversationDTO.IsProvidedNumOfPassenger)
			{
				internalContext += $@"
Trước tiên, hãy hỏi người dùng câu hỏi sau:

**Bạn vui lòng cung cấp thông tin về số lượng hành khách, cụ thể là số lượng người lớn, trẻ em và em bé được không?**

Quy tắc xử lý:

1. Nếu người dùng chưa cung cấp thông tin về số lượng hành khách, hãy **lặp lại câu hỏi trên** để hỏi lại.
2. Nếu người dùng chỉ cung cấp một phần thông tin (ví dụ chỉ nói số lượng người lớn), hãy trả lời:  
   **“Thông tin bạn cung cấp chưa đầy đủ”**, và hỏi lại câu trên.
3. Nếu tổng số lượng hành khách vượt quá 9, hãy trả lời:  
   **“Giới hạn số lượng hành khách trong hệ thống không được lớn hơn 9 người (bao gồm người lớn, trẻ em và em bé).”**  
   Sau đó, hỏi lại câu hỏi ban đầu.

**Lưu ý đặc biệt:** Nếu người dùng đã từng cung cấp một phần thông tin, hãy **lưu tạm dữ liệu** đó và sử dụng khi hỏi lại, để không làm mất thông tin trước đó.

---

Nếu thông tin về số lượng hành khách đã đầy đủ và hợp lệ (tổng ≤ 9), hãy phản hồi lại người dùng theo định dạng:

Dưới đây là thông tin số lượng mà bạn đã cung cấp:
- Số lượng người lớn: [giá trị]
- Số lượng trẻ em: [giá trị]
- Số lượng em bé: [giá trị]

Sau đó hãy **xóa dữ liệu tạm đã lưu** và không sử dụng lại nữa.
";
			}

			internalContext += geminiConversationDTO.NumAdult > 0 ? $@"
	 Số lượng hành khách người lớn đã khai báo: {geminiConversationDTO.NumAdult}" : "";
			internalContext += geminiConversationDTO.NumChild > 0 ? $@"
	 Số lượng hành khách trẻ em đã khai báo: {geminiConversationDTO.NumChild}" : "";
			internalContext += geminiConversationDTO.NumInfant > 0 ? $@"
	 Số lượng hành khách trẻ em đã khai báo: {geminiConversationDTO.NumInfant}" : "";
			if (!geminiConversationDTO.IsSearchFlight)
			{
				internalContext += $@"
Người dùng có thể chọn một hoặc nhiều tiêu chí tìm kiếm chuyến bay từ các tùy chọn sau:

1. **Điểm đi**  
   - 1. Hà Nội (HAN)
- 2. Hồ Chí Minh (SGN)
- 3. Đà Nẵng (DNG)
- 4. Phú Quốc (PQA)
- 5. Nha Trang (CRA)
2. **Điểm đến**  
   - 1. Hà Nội (HAN)
- 2. Hồ Chí Minh (SGN)
- 3. Đà Nẵng (DNG)
- 4. Phú Quốc (PQA)
- 5. Nha Trang (CRA)
3. **Giờ cất cánh** (Chọn một hoặc nhiều khoảng sau):
   - Từ 0h đến 6h  
   - Từ 6h đến 12h  
   - Từ 12h đến 18h  
   - Từ 18h đến 0h  
4. **Giờ hạ cánh** (Chọn một hoặc nhiều khoảng sau):  
   - Từ 0h đến 6h  
   - Từ 6h đến 12h  
   - Từ 12h đến 18h  
   - Từ 18h đến 0h  
5. **Ngày diễn ra chuyến bay**  
6. **Hãng máy bay** (Chỉ hỗ trợ):  
   - Bamboo  
   - Boeing  
7. **Giá cả** (Chọn một hoặc nhiều mức sau):  
   - 250.000 - 2.500.000 VND  
   - 2.500.000 - 5.000.000 VND  
   - 5.000.000 - 7.500.000 VND  
   - 7.500.000 - 10.000.000 VND  

---

**Hướng dẫn người dùng**:
- Bạn có thể chọn **nhiều tiêu chí cùng lúc** và nhập thông tin **trực tiếp** trong một câu trả lời.
- Ví dụ:  
  “Tôi muốn bay từ Hà Nội đến Sài Gòn vào ngày 25/07 lúc 9h sáng, hãng Bamboo, giá dưới 2 triệu.”

---

**Cách xử lý**:
- Phân tích câu trả lời của người dùng, xác định thông tin tương ứng với các tiêu chí (từ 1 đến 7).
- Nếu hợp lệ thì cung cấp thông tin theo các tiêu chí trên.
- **Chỉ chấp nhận giá cả và giờ bay nằm trong danh sách đã cho.** Nếu người dùng nhập không đúng khoảng, hãy yêu cầu chọn lại theo đúng định dạng.
- **Không hỏi lại những thông tin đã rõ ràng.** Chỉ hỏi lại các phần còn thiếu hoặc không hợp lệ.
- **Hiển thị ngày diễn ra chuyến bay** đúng định dạng. Ví dụ: 2025-07-25, 2023-10-11,...

---

**Yêu cầu quan trọng khi phản hồi**:
- Sau khi trích xuất được các thông tin từ người dùng, hãy hiển thị lại **chỉ những tiêu chí đã được cung cấp** theo format sau:

- Điểm đi: [giá trị nếu có trong danh sách điểm đi ở trên và là giá trị số ứng với số được đánh trong các điểm: Ví dụ: 1]  
- Điểm đến: [giá trị nếu có trong danh sách điểm đến ở trên và là giá trị số ứng với số được đánh trong các điểm: Ví dụ: 1]   
- Giờ cất cánh: [nếu có nhiều khoảng thì cách nhau bằng dấu phẩy: Ví dụ: 'Từ 0h đến 6h, Từ 6h đến 12h']  
- Giờ hạ cánh: [nếu có nhiều khoảng thì cách nhau bằng dấu phẩy]  
- Ngày diễn ra chuyến bay: [giá trị theo định dạng yyyy-MM-dd, Ví dụ: 2023-10-11, 2025-07-25,...]
- Hãng máy bay: [giá trị nếu có]  
- Giá cả: [nếu có nhiều mức giá thì cách nhau bằng dấu chấm: Ví dụ: '250.000 - 2.500.000 VND, 2.500.000 - 5.000.000 VND']

- **Không hiển thị các tiêu chí mà người dùng chưa cung cấp.**
- Dựa vào các thông tin đã có, phản hồi kèm thông báo chúng tôi sẽ tìm kiếm hàm đấy dựa trên điều kiện này
**Lưu ý**: Người dùng có thể nhập giờ hoặc giá theo nhiều khoảng, ví dụ:
- “Giờ bay từ 6h đến 12h và 12h đến 18h”
- “Giá từ 250.000 đến 5 triệu”

→ Trong trường hợp này, hãy hiểu người dùng chọn nhiều khoảng giá hoặc khung giờ và xử lý tương ứng.
";
			}

			if (!geminiConversationDTO.IsSelectedFlight && geminiConversationDTO.IsSearchFlight)
			{

				internalContext += $@"
Sau khi đã lọc được danh sách các chuyến bay dựa trên điều kiện mà người dùng cung cấp (nếu có), hãy hỏi người dùng muốn chọn chuyến bay nào.

- Nếu có nhiều chuyến bay phù hợp: hiển thị danh sách theo định dạng sau(**bắt buộc**).
- Danh sách chuyến bay theo yêu cầu của quý vị:
   - Id chuyến bay: [id chuyến bay]
                - Mã chuyến bay: [mã chuyến bay]
				  - Từ: [giá trị id của điểm đi] ([mã code điểm đi]) đến [giá trị id của điểm đến] ([mã code điểm đến])
				  - Thời gian: [thời gian cất cánh] - [thời gian hạ cánh]
				  - Thông tin máy bay: 
					+ Mã máy bay: [mã máy bay]
					+ Nhà sản xuất: [nhà sản xuất]
				  - Số lượng hạng ghế: [số lượng hạng ghế]
				  - Phí vận chuyển: [phí vận chuyển]
				  - Giá cả: [giá cả chuyến bay]

và mời người dùng lựa chọn một trong số chúng
- Nếu không có chuyến bay nào thỏa mãn điều kiện: thông báo không tìm thấy và đề nghị người dùng thử lại với tiêu chí khác (như thay đổi thời gian, điểm đến, mức giá...).

Sau khi người dùng đã chọn chuyến bay, nếu chuyến bay đó tồn tại trong danh sách, hãy hiển thị thông tin theo định dạng sau:

Thông tin chuyến bay quý khách đã chọn:  
 - Mã chuyến bay: [mã chuyến bay]  
 - Id chuyến bay: [id chuyến bay]
 - Từ: [mã code điểm đi] đến [mã code điểm đến]  
 - Thời gian: [thời gian đi] - [thời gian đến]  
 - Thông tin máy bay:  
	+ Mã máy bay: [mã máy bay]  
	+ Nhà sản xuất: [nhà sản xuất]  
 - Số lượng hạng ghế: [số lượng hạng ghế]  
 - Phí vận chuyển: [phí vận chuyển]  
 - Giá cả: [giá cả]  
 Đồng thời hiển thị thêm thông tin sau(nếu có):
Nếu có đoạn **Số lượng hành khách người lớn đã khai báo**, hiển thị thêm thông tin theo định dạng sau:
- Thông tin cho hành khách người lớn:
	+ Số lượng: [**Số lượng hành khách người lớn đã khai báo**]
	+ Tổng tiền chuyến bay: [**Số lượng hành khách người lớn đã khai báo** nhân với **giá cả chuyến bay**]

Nếu có đoạn **Số lượng hành khách trẻ em đã khai báo**, hiển thị thêm thông tin theo định dạng sau:
- Thông tin cho hành khách trẻ em:
	+ Số lượng: [**Số lượng hành khách trẻ em đã khai báo**]
	+ Tổng tiền chuyến bay: [**Số lượng hành khách trẻ em đã khai báo** nhân với **giá cả chuyến bay** nhân với 0.5]
";
				internalContext += $@"
Nếu chuyến bay người dùng chọn không tồn tại, hãy thông báo và mời người dùng chọn lại hoặc thay đổi tiêu chí tìm kiếm.
Lưu ý: luôn sử dụng dữ liệu chuyến bay hệ thống đã nêu ở trên để phản hồi.
";
			}

			if (!geminiConversationDTO.IsSelectedClassSeat)
			{
				internalContext += $@"
Hiển thị danh sách hạng ghế theo định dạng sau :
- Dưới đây là danh sách hạng ghế theo chuyến bay quý vị đã chọn:
- Id hạng ghế: [id hạng ghế]
- Tên hạng ghế: [tên hạng ghế]
- Giá hạng ghế: [giá hạng ghế]

và hãy hỏi người dùng muốn chọn hạng ghế nào.

- Nếu có nhiều chuyến bay phù hợp: hiển thị danh sách và mời người dùng lựa chọn một trong số đó.
- Nếu không có hạng ghế nào thỏa mãn điều kiện: thông báo không tìm thấy và đề nghị người dùng chọn lại hạng ghế hoặc tìm kiếm lại chuyến bay.

Sau khi người dùng đã chọn hạng ghế, nếu hạng ghế đó tồn tại trong danh sách, hãy hiển thị thông tin theo định dạng sau:

Thông tin hạng ghế quý khách đã chọn:  
 - Id hạng ghế: [id hạng ghế]
 - Tên hạng ghế: [tên hạng ghế]
 - Giá hạng ghế: [giá hạng ghế]

và hỏi như sau **Bạn có xác nhận có muốn đặt chuyến bay này không?**
";
				internalContext += $@"
Nếu hạng ghế người dùng chọn không tồn tại, hãy thông báo **Hạng ghế không tồn tại trong hệ thống** và mời người dùng chọn lại hoặc tìm kiếm lại chuyến bay theo các tiêu chí như ban đầu.
Lưu ý: luôn sử dụng dữ liệu hạng ghế hệ thống đã nêu ở trên để phản hồi.
";
			}
			else
			{
				internalContext += $@"
Hỏi người dùng có muốn đặt chuyến bay này không. Nếu có trả lời **Bạn đã đặt chuyến bay này. Xin vui lòng chờ một chút để chúng tôi xác nhận thông tin**
Nếu không trả lời **Bạn đã hủy đặt chuyến bay này. Xin lỗi về sự bất tiện này** và yêu cầu người dùng tìm kiếm lại chuyến bay theo tiêu chí như ở trên
				";
			}




				var finalPrompt = internalContext + "\n\nCâu hỏi người dùng:\n" + geminiConversationDTO.Prompt;

			var requestData = new
			{
				contents = new[] {
			new {
				parts = new[] {
					new { text = finalPrompt }
				}
			}
				}
			};
			var response = await _httpClient.PostAsJsonAsync(requestUrl, requestData);
			var result = await response.Content.ReadFromJsonAsync<JsonElement>();

			if (result.TryGetProperty("candidates", out var candidates)
				&& candidates.ValueKind == JsonValueKind.Array
				&& candidates.GetArrayLength() > 0)
			{
				var candidate = candidates[0];

				if (candidate.TryGetProperty("content", out var content)
					&& content.ValueKind == JsonValueKind.Object
					&& content.TryGetProperty("parts", out var parts)
					&& parts.ValueKind == JsonValueKind.Array
					&& parts.GetArrayLength() > 0)
				{
					var part = parts[0];

					if (part.TryGetProperty("text", out var text)
						&& text.ValueKind == JsonValueKind.String)
					{
						return text.GetString() ?? string.Empty;
					}
				}
			}
			return string.Empty;

		}

		public async Task<List<dynamic>> GetAllFlightSeatWithGeminiByFlightId(int flightId)
		{
			return await _flightService.GetAllClassSeatByFlightIdAndSeatEmpty(flightId);
		}

		public async Task<List<FlightQueryDTO>> GetFlightsWithGemini(FilterFlightDTO filterFlightDTO)
		{
			return await _flightService.GetAllFlightsWithGemini(filterFlightDTO);
		}
	}
}
