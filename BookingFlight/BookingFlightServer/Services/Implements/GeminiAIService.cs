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
    }
}
