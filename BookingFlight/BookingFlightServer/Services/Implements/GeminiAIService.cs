using BookingFlightServer.Services;
using System.Text;
using System.Text.Json;

namespace BookingFlightServer.Services.Implements
{
    public class GeminiAIService : IGeminiAIService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GeminiAIService> _logger;
        private readonly string _apiKey;

        public GeminiAIService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<GeminiAIService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _apiKey = configuration["GeminiAI:ApiKey"] ?? throw new ArgumentNullException("GeminiAI:ApiKey not configured");
        }

        public async Task<string> GenerateContentAsync(string prompt)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiKey))
                {
                    return "AI service is not properly configured.";
                }

                var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}";

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

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    if (result.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                    {
                        var firstCandidate = candidates[0];
                        if (firstCandidate.TryGetProperty("content", out var contentProp) &&
                            contentProp.TryGetProperty("parts", out var parts) && parts.GetArrayLength() > 0)
                        {
                            var firstPart = parts[0];
                            if (firstPart.TryGetProperty("text", out var textProp))
                            {
                                return textProp.GetString() ?? "No response generated.";
                            }
                        }
                    }
                }

                return "Unable to generate response from AI service.";
            }
            catch (Exception ex)
            {
                return "An error occurred while generating AI response.";
            }
        }

        public async Task<bool> IsComplaintRelevantAsync(string complaintDescription)
        {
            try
            {
                // If complaint is empty or too short, consider it relevant to avoid auto-rejection
                if (string.IsNullOrWhiteSpace(complaintDescription) || complaintDescription.Length < 5)
                {
                    return true; // Let supporter handle it manually
                }

                var prompt = $@"Analyze this customer complaint and determine if it's related to airline services.

Complaint: ""{complaintDescription}""

Respond 'RELEVANT' if the complaint mentions:
- Flight booking, cancellation, changes
- Flight delays, cancellations, schedule changes  
- Baggage, check-in, airport services
- Seating, upgrades, in-flight amenities
- Refunds, payments related to flights
- Customer service about aviation
- Flight safety, procedures
- Booking website, flight booking app
- ANY issue that could be aviation-related
- Customer experience with airline services
- Food, beverages, entertainment on flights
- Staff behavior on flights or at airport

Respond 'IRRELEVANT' ONLY if:
- Clearly spam advertising
- Completely unrelated to aviation (e.g., restaurants, cars not related to travel)
- Seriously offensive content
- Meaningless content (just random characters)
- Obvious test messages like 'test', 'hello', '123'

WHEN IN DOUBT, CHOOSE 'RELEVANT' for manual review by staff.

Response:";

                var response = await GenerateContentAsync(prompt);
                var cleanResponse = response.Trim().ToUpper();
                
                // More liberal matching - default to relevant unless clearly irrelevant
                bool isRelevant = cleanResponse.Contains("RELEVANT") || 
                       cleanResponse.Contains("CÓ LIÊN QUAN") ||
                       cleanResponse.Contains("YES") || 
                       cleanResponse.Contains("RELATED") ||
                       !cleanResponse.Contains("IRRELEVANT") &&
                       !cleanResponse.Contains("KHÔNG LIÊN QUAN");
                
                _logger.LogInformation($"AI relevance check for complaint: '{complaintDescription}' -> {(isRelevant ? "RELEVANT" : "IRRELEVANT")}");
                
                return isRelevant;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AI relevance check");
                // If AI fails, default to relevant to avoid auto-rejection
                return true;
            }
        }

        public async Task<string> GenerateRejectionReasonAsync(string complaintDescription)
        {
            try
            {
                var prompt = $@"Generate a professional, polite rejection message for this customer complaint:

Complaint: ""{complaintDescription}""

The rejection reason should:
1. Be professional and respectful
2. Explain why the complaint cannot be processed
3. Suggest alternative actions if appropriate
4. Keep it concise (2-3 sentences)
5. Be suitable for customer communication

Please provide only the rejection message without any additional formatting or labels.";

                var response = await GenerateContentAsync(prompt);
                
                var cleanResponse = response.Trim();
                cleanResponse = cleanResponse.Trim('"', '\n', '\r', ' ');
                
                return string.IsNullOrWhiteSpace(cleanResponse) 
                    ? "Your complaint has been reviewed but cannot be processed as it falls outside our service scope. Please contact our general customer service for further assistance."
                    : cleanResponse;
            }
            catch (Exception ex)
            {
                return "Your complaint has been rejected due to policy violations or insufficient information. Please provide more details or contact support for further assistance.";
            }
        }

        public async Task<string> AnalyzeFlightConflicts(string conflictData)
        {
            try
            {
                var prompt = $@"Analyze the following flight conflict data and provide recommendations:
{conflictData}

Please provide:
1. Summary of conflicts found
2. Recommended resolution steps
3. Priority level (High/Medium/Low)";

                return await GenerateContentAsync(prompt);
            }
            catch (Exception ex)
            {
                return "Unable to analyze flight conflicts at this time.";
            }
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
                var departureChange = (newDepartureTime - oldDepartureTime).TotalMinutes;
                var arrivalChange = (newArrivalTime - oldArrivalTime).TotalMinutes;

                var prompt = $@"Generate a professional flight schedule change notification for flight {flightCode}.
Old departure: {oldDepartureTime:yyyy-MM-dd HH:mm}
New departure: {newDepartureTime:yyyy-MM-dd HH:mm}
Change: {departureChange:+0;-0;0} minutes

Old arrival: {oldArrivalTime:yyyy-MM-dd HH:mm}
New arrival: {newArrivalTime:yyyy-MM-dd HH:mm}
Change: {arrivalChange:+0;-0;0} minutes

Please provide a brief, professional explanation (2-3 sentences) for this schedule change that could be sent to passengers.";

                return await GenerateContentAsync(prompt);
            }
            catch (Exception ex)
            {
                return "Flight schedule has been updated due to operational requirements. We apologize for any inconvenience caused.";
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
                var contextInfo = additionalContext != null
                    ? string.Join(", ", additionalContext.Select(kv => $"{kv.Key}: {kv.Value}"))
                    : "No additional context";

                var prompt = $@"Generate a professional explanation for a flight change:
Flight: {flightCode}
Change Type: {changeType}
Old Value: {oldValue}
New Value: {newValue}
Additional Context: {contextInfo}

Please provide a brief, professional explanation (1-2 sentences) suitable for passenger communication.";

                return await GenerateContentAsync(prompt);
            }
            catch (Exception ex)
            {
                return changeType.ToLower() switch
                {
                    "time" => "Flight time has been adjusted due to operational requirements.",
                    "gate" => "Gate assignment has been changed for operational efficiency.",
                    "aircraft" => "Aircraft assignment has been updated for this flight.",
                    _ => "Flight details have been updated due to operational requirements."
                };
            }
        }
    }
}
