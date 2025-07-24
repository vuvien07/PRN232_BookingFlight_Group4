using BookingFlightServer.DTO.Request;
using BookingFlightServer.DTO.Response;
using BookingFlightServer.Entities;
namespace BookingFlightServer.Repositories
{
	public interface IFeedbackRepository
	{
		Task<double> getAverageFeedback();
		Task<List<Feedback>?> getFeedbackByFilter(FilterFeedbackDTO filterFeedbackDTO);
		Task<Feedback> CreateAsync(Feedback feedback);
		Task<List<Feedback>> GetAllAsync();
		Task<List<Feedback>> GetFeedbacksByAccountIdAsync(int accountId);
		Task<List<Feedback>> GetFeedbacksByTicketIdAsync(int ticketId);
		Task<Feedback?> GetByIdAsync(int id);
		Task<bool> HasCustomerAlreadyFeedback(int accountId);
		Task<Feedback> UpdateAsync(Feedback feedback);
		Task<bool> DeleteAsync(int id);
		
		// Join gián tiếp qua Account → Customer → Ticket → Flight
		Task<List<FeedbackDetailDTO>> GetFeedbacksDetailByTicketIdAsync(int ticketId);
		Task<List<FeedbackDetailDTO>> GetFeedbacksDetailByAccountIdAsync(int accountId);
		Task<List<FeedbackDetailDTO>> GetAllFeedbackDetailsAsync();
		Task<List<FeedbackDetailDTO>> SearchAndFilterFeedbacksAsync(string? searchTerm = null, int? rating = null, DateTime? fromDate = null, DateTime? toDate = null);
		Task<List<FeedbackDetailDTO>> GetFeedbackDetailsByTicketIdAndAccountIdAsync(int ticketId, int accountId);
	}
}
