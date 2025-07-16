namespace BookingFlightServer.Services
{
    public interface IGeminiAIService
    {
        Task<string> AnalyzeFlightConflicts(string conflictData);
    }
}
