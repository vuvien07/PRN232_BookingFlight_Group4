using BookingFlightServer.DTO.Request;
using BookingFlightServer.DTO.Response;

namespace BookingFlightServer.Services
{
	public interface IFeedbackService
	{
		Task<bool> CreateFeedbackAsync(CreateFeedbackDTO createFeedbackDTO, int accountId);
		Task<List<FeedbackDTO>> GetAllFeedbacksAsync();
		Task<List<FeedbackDTO>> GetFeedbacksByAccountIdAsync(int accountId);
		Task<List<FeedbackDTO>> GetFeedbacksByTicketIdAsync(int ticketId);
		Task<List<FeedbackDetailDTO>> GetFeedbacksDetailByTicketIdAsync(int ticketId);
		Task<List<FeedbackDetailDTO>> GetFeedbacksDetailByAccountIdAsync(int accountId);
		Task<List<FeedbackDetailDTO>> GetAllFeedbackDetailsAsync();
		Task<List<FeedbackDetailDTO>> SearchAndFilterFeedbacksAsync(string? searchTerm = null, int? rating = null, DateTime? fromDate = null, DateTime? toDate = null);
		Task<bool> HasFeedbackByTicketIdAsync(int ticketId, int accountId);
		Task<List<FeedbackDetailDTO>> GetFeedbackDetailsByTicketIdAsync(int ticketId, int accountId);
		Task<List<FeedbackDetailDTO>> GetFeedbackDetailsByTicketIdAsync(int ticketId);
	}
}
