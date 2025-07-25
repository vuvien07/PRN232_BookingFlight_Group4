namespace BookingFlightServer.Services
{
    public interface IFeedbackTicketMappingService
    {
        // Simple mapping interface for feedback ticket relationships
        Task<List<int>> GetFeedbackIdsByTicketId(int ticketId);
        Task<int?> GetTicketIdByFeedbackId(int feedbackId);
        Task<bool> HasFeedbackForTicketAndAccount(int ticketId, int accountId);
        Task<bool> LinkFeedbackToTicketAndAccount(int feedbackId, int ticketId, int accountId);
    }
}