using BookingFlightServer.DTO.Request;
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
		Task<Feedback?> GetByIdAsync(int id);
		Task<bool> HasCustomerAlreadyFeedback(int accountId);
		Task<Feedback> UpdateAsync(Feedback feedback);
		Task<bool> DeleteAsync(int id);
	}
}
