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
                    _logger.LogInformation($"Short complaint considered RELEVANT: '{complaintDescription}'");
                    return true; // Let supporter handle it manually
                }

                // Check for obvious random/gibberish patterns first
                if (IsObviousGibberish(complaintDescription))
                {
                    _logger.LogInformation($"Obvious gibberish detected, marking as IRRELEVANT: '{complaintDescription}'");
                    return false;
                }

                // Check for obvious spam patterns
                var lowerDescription = complaintDescription.ToLower();
                var obviousSpamPatterns = new[] {
                    "buy now", "click here", "free money", "win money", "lottery", "casino",
                    "viagra", "pharmacy", "pills", "weight loss", "make money",
                    "http://", "https://", "www.", ".com", ".net", ".org"
                };

                var hasSpamPattern = obviousSpamPatterns.Any(pattern => lowerDescription.Contains(pattern));
                
                // If it contains obvious spam, check with AI
                if (hasSpamPattern)
                {
                    _logger.LogInformation($"Potential spam detected, checking with AI: '{complaintDescription}'");
                }

                var prompt = $@"Analyze this customer complaint for an airline and determine if it should be processed.

Complaint: ""{complaintDescription}""

ALWAYS respond 'RELEVANT' unless the complaint is:
1. OBVIOUS advertising spam (selling products, services, contains URLs)
2. COMPLETELY unrelated to travel/aviation (e.g., cooking recipes, car repairs)
3. PURELY random characters with no meaning (e.g., 'asdfghjkl', '123456789', random letters)
4. CLEARLY offensive or abusive language
5. Gibberish or nonsensical text that has no meaning

Consider RELEVANT (accept for human review):
- ANY mention of flights, booking, airlines, travel
- Customer service complaints 
- Any dissatisfaction with service
- Pricing concerns
- Website/app issues
- Food, seating, baggage complaints
- ANY aviation-related concern
- Even vague complaints that might relate to travel
- Personal experiences during travel
- Suggestions or feedback
- Questions about flights or services

WHEN IN ANY DOUBT, CHOOSE 'RELEVANT' - it's better to let humans review than auto-reject.

Response (RELEVANT or IRRELEVANT):";

                var response = await GenerateContentAsync(prompt);
                var cleanResponse = response.Trim().ToUpper();
                
                // VERY liberal matching - default to RELEVANT unless explicitly IRRELEVANT
                bool isRelevant = !cleanResponse.Contains("IRRELEVANT") || 
                                 cleanResponse.Contains("RELEVANT") ||
                                 cleanResponse.Contains("CÓ LIÊN QUAN") ||
                                 cleanResponse.Contains("YES") || 
                                 cleanResponse.Contains("RELATED");
                
                _logger.LogInformation($"AI relevance check for complaint: '{complaintDescription}' -> {(isRelevant ? "RELEVANT" : "IRRELEVANT")} (AI response: '{cleanResponse}')");
                
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

        private bool IsObviousGibberish(string text)
        {
            // Remove spaces and common punctuation for analysis
            var cleanText = text.Replace(" ", "").Replace(".", "").Replace(",", "").Replace("!", "").Replace("?", "");
            
            // Check if text is too short to be meaningful
            if (cleanText.Length < 5)
                return false; // Too short to determine, let AI decide
            
            // Count consonant clusters (more than 4 consonants in a row is suspicious)
            var consonantClusterPattern = @"[bcdfghjklmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ]{5,}";
            if (System.Text.RegularExpressions.Regex.IsMatch(cleanText, consonantClusterPattern))
            {
                _logger.LogInformation($"Detected long consonant cluster in: '{text}'");
                return true;
            }
            
            // Check for patterns of alternating random characters
            var randomPatterns = new[]
            {
                @"[A-Z]{3,}\s[A-Z]{3,}\s[A-Z]{3,}", // Multiple uppercase words like "ABC DEF GHI"
                @"[bcdfghjklmnpqrstvwxyz]{6,}", // Long strings of consonants
                @"^[A-Z\s]*$", // Only uppercase letters and spaces (like the example)
            };
            
            foreach (var pattern in randomPatterns)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(text, pattern) && 
                    !ContainsCommonWords(text))
                {
                    _logger.LogInformation($"Detected gibberish pattern '{pattern}' in: '{text}'");
                    return true;
                }
            }
            
            // Check ratio of vowels to consonants (normal text should have reasonable vowel ratio)
            var vowels = cleanText.Count(c => "aeiouAEIOU".Contains(c));
            var letters = cleanText.Count(char.IsLetter);
            
            if (letters > 10 && vowels == 0)
            {
                _logger.LogInformation($"No vowels detected in text with {letters} letters: '{text}'");
                return true;
            }
            
            return false;
        }
        
        private bool ContainsCommonWords(string text)
        {
            var commonWords = new[] { 
                "flight", "plane", "ticket", "booking", "airport", "travel", "customer", "service",
                "complaint", "problem", "issue", "help", "support", "bad", "good", "cancel",
                "delay", "late", "early", "seat", "food", "baggage", "staff", "price", "cost",
                "chuyến bay", "máy bay", "vé", "đặt vé", "sân bay", "du lịch", "khách hàng",
                "dịch vụ", "khiếu nại", "vấn đề", "giúp đỡ", "hỗ trợ", "tệ", "tốt", "hủy",
                "trễ", "sớm", "ghế ngồi", "đồ ăn", "hành lý", "nhân viên", "giá", "chi phí"
            };
            
            var lowerText = text.ToLower();
            return commonWords.Any(word => lowerText.Contains(word));
        }
    }
}
