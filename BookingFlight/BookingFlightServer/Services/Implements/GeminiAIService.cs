using BookingFlightServer.Services;
using System.Text;
using System.Text.Json;

namespace BookingFlightServer.Services.Implements
{
    public class GeminiAIService : IGeminiAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiUrl;

        public GeminiAIService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GeminiAI:ApiKey"];
            _apiUrl = configuration["GeminiAI:ApiUrl"];
        }

        public async Task<string> GenerateFlightUpdateReasonAsync(
            string flightCode, 
            DateTime oldDepartureTime, 
            DateTime newDepartureTime,
            DateTime oldArrivalTime, 
            DateTime newArrivalTime)
        {
            try
            {
                var prompt = BuildPrompt(flightCode, oldDepartureTime, newDepartureTime, oldArrivalTime, newArrivalTime);
                
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiUrl}?key={_apiKey}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return ParseGeminiResponse(responseContent);
                }

                return "Thay đổi lịch trình do yêu cầu điều phối bay và đảm bảo an toàn cho chuyến bay.";
            }
            catch (Exception ex)
            {
                return $"Do điều kiện thời tiết và yêu cầu an toàn bay, chúng tôi cần điều chỉnh lịch trình chuyến bay. Chi tiết: {ex.Message}";
            }
        }

        private string BuildPrompt(string flightCode, DateTime oldDepartureTime, DateTime newDepartureTime, DateTime oldArrivalTime, DateTime newArrivalTime)
        {
            var currentDate = DateTime.Now.ToString("dd/MM/yyyy");
            return $@"
Bạn là một chuyên gia hàng không. Hãy tạo ra một lý do hợp lý và chuyên nghiệp cho việc thay đổi lịch trình chuyến bay {flightCode}.

Thông tin thay đổi:
- Ngày hiện tại: {currentDate}
- Giờ khởi hành cũ: {oldDepartureTime:dd/MM/yyyy HH:mm}
- Giờ khởi hành mới: {newDepartureTime:dd/MM/yyyy HH:mm}
- Giờ đến cũ: {oldArrivalTime:dd/MM/yyyy HH:mm}
- Giờ đến mới: {newArrivalTime:dd/MM/yyyy HH:mm}

Hãy tạo ra một lý do ngắn gọn (1-2 câu) về việc thay đổi này. Lý do phải:
1. Chuyên nghiệp và đáng tin cậy
2. Liên quan đến an toàn bay, thời tiết, hoặc điều phối kỹ thuật
3. Thể hiện sự quan tâm đến hành khách
4. Viết bằng tiếng Việt

Chỉ trả về lý do, không cần giải thích thêm.";
        }

        private string ParseGeminiResponse(string responseContent)
        {
            try
            {
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                
                if (jsonResponse.TryGetProperty("candidates", out var candidates) && 
                    candidates.GetArrayLength() > 0)
                {
                    var firstCandidate = candidates[0];
                    if (firstCandidate.TryGetProperty("content", out var contentProp) &&
                        contentProp.TryGetProperty("parts", out var parts) &&
                        parts.GetArrayLength() > 0)
                    {
                        var firstPart = parts[0];
                        if (firstPart.TryGetProperty("text", out var text))
                        {
                            var result = text.GetString()?.Trim();
                            return !string.IsNullOrEmpty(result) ? result : "Thay đổi lịch trình do yêu cầu điều phối bay và đảm bảo an toàn cho chuyến bay.";
                        }
                    }
                }

                return "Thay đổi lịch trình do yêu cầu điều phối bay và đảm bảo an toàn cho chuyến bay.";
            }
            catch
            {
                return "Thay đổi lịch trình do yêu cầu điều phối bay và đảm bảo an toàn cho chuyến bay.";
            }
        }

        public async Task<string> AnalyzeFlightConflicts(string conflictData)
        {
            try
            {
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new
                                {
                                    text = $"Analyze the following flight conflict data and provide recommendations:\n\n{conflictData}\n\nPlease provide:\n1. Summary of conflicts\n2. Recommendations to resolve conflicts\n3. Alternative scheduling suggestions"
                                }
                            }
                        }
                    }
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiUrl}?key={_apiKey}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    
                    if (jsonResponse.TryGetProperty("candidates", out var candidates) && 
                        candidates.GetArrayLength() > 0)
                    {
                        var firstCandidate = candidates[0];
                        if (firstCandidate.TryGetProperty("content", out var contentProp) &&
                            contentProp.TryGetProperty("parts", out var parts) &&
                            parts.GetArrayLength() > 0)
                        {
                            var firstPart = parts[0];
                            if (firstPart.TryGetProperty("text", out var text))
                            {
                                return text.GetString() ?? "No analysis available";
                            }
                        }
                    }
                }

                return "Unable to analyze conflicts at this time";
            }
            catch (Exception ex)
            {
                return $"Error analyzing conflicts: {ex.Message}";
            }
        }

        public async Task<string> GenerateFlightChangeReasonAsync(
            string flightCode,
            string changeType,
            object oldValue,
            object newValue,
            Dictionary<string, object>? additionalContext = null)
        {
            try
            {
                // Check if API key and URL are configured
                if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_apiUrl))
                {
                    Console.WriteLine("GeminiAI: API key or URL not configured, using default reason");
                    return GetDefaultReasonForChangeType(changeType);
                }

                var prompt = BuildAdvancedPrompt(flightCode, changeType, oldValue, newValue, additionalContext);
                Console.WriteLine($"GeminiAI: Sending prompt for {changeType}: {prompt.Substring(0, Math.Min(200, prompt.Length))}...");
                
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiUrl}?key={_apiKey}", content);
                
                Console.WriteLine($"GeminiAI: Response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"GeminiAI: Response content: {responseContent.Substring(0, Math.Min(500, responseContent.Length))}...");
                    var result = ParseGeminiResponse(responseContent);
                    Console.WriteLine($"GeminiAI: Parsed result: {result}");
                    return result;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"GeminiAI: Error response: {errorContent}");
                    return GetDefaultReasonForChangeType(changeType);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GeminiAI: Exception occurred: {ex.Message}");
                return GetDefaultReasonForChangeType(changeType) + $" (Error: {ex.Message})";
            }
        }

        private string BuildAdvancedPrompt(string flightCode, string changeType, object oldValue, object newValue, Dictionary<string, object>? additionalContext)
        {
            var currentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            var prompt = $@"
Bạn là một chuyên gia hàng không và dịch vụ khách hàng. Hãy tạo ra một lý do hợp lý và chuyên nghiệp cho việc thay đổi thông tin chuyến bay {flightCode}.

Thời gian hiện tại: {currentDate}
Loại thay đổi: {changeType}
Giá trị cũ: {oldValue}
Giá trị mới: {newValue}";

            if (additionalContext?.Any() == true)
            {
                prompt += "\nThông tin bổ sung:";
                foreach (var context in additionalContext)
                {
                    prompt += $"\n- {context.Key}: {context.Value}";
                }
            }

            prompt += $@"

Hãy tạo ra một lý do ngắn gọn và cụ thể (1-3 câu) cho việc thay đổi {changeType} này. Lý do phải:

1. Chuyên nghiệp và đáng tin cậy
2. Cụ thể theo loại thay đổi:
   - Nếu là thay đổi thời gian: liên quan đến thời tiết, an toàn bay, điều phối không lưu
   - Nếu là thay đổi máy bay: liên quan đến bảo trì, nâng cấp, điều phối kỹ thuật
   - Nếu là thay đổi thuế/phí: liên quan đến chính sách mới, quy định hàng không
   - Nếu là thay đổi cổng/terminal: liên quan đến điều phối sân bay, tối ưu hóa
3. Thể hiện sự quan tâm đến hành khách
4. Viết bằng tiếng Việt, giọng điệu lịch sự và chuyên nghiệp
5. Đưa ra thông tin cụ thể và thực tế

Chỉ trả về lý do, không cần giải thích thêm.";

            return prompt;
        }

        private string GetDefaultReasonForChangeType(string changeType)
        {
            return changeType.ToLower() switch
            {
                "tax" or "thuế" => "Do cập nhật chính sách thuế và phí hàng không mới từ cơ quan quản lý, chúng tôi cần điều chỉnh mức thuế áp dụng cho chuyến bay này.",
                "aircraft" or "máy bay" => "Do yêu cầu bảo trì định kỳ và đảm bảo an toàn kỹ thuật, chúng tôi thực hiện thay đổi loại máy bay cho chuyến bay này.",
                "gate" or "cổng" => "Do điều phối tối ưu hóa hoạt động sân bay và đảm bảo đúng giờ, chúng tôi thay đổi cổng khởi hành cho chuyến bay này.",
                "terminal" => "Do cải thiện dịch vụ và thuận tiện cho hành khách, chúng tôi điều chỉnh terminal cho chuyến bay này.",
                "price" or "giá" => "Do cập nhật chính sách giá vé và điều kiện thị trường, chúng tôi thực hiện điều chỉnh giá vé cho chuyến bay này.",
                _ => "Do yêu cầu điều phối và đảm bảo chất lượng dịch vụ tốt nhất, chúng tôi thực hiện thay đổi thông tin chuyến bay này."
            };
        }
    }
}
