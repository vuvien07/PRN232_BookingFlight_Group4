namespace BookingFlightServer.Services
{
    public interface IGeminiAIService
    {
        Task<string> AnalyzeFlightConflicts(string conflictData);
        Task<string> GenerateFlightUpdateReasonAsync(
            string flightCode, 
            DateTime oldDepartureTime, 
            DateTime newDepartureTime,
            DateTime oldArrivalTime, 
            DateTime newArrivalTime);
        Task<string> GenerateFlightChangeReasonAsync(
            string flightCode,
            string changeType,
            object oldValue,
            object newValue,
            Dictionary<string, object>? additionalContext = null);
    }
}
