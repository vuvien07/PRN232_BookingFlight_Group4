using BookingFlightServer.DTO.Request;
using BookingFlightServer.DTO.Response;

namespace BookingFlightServer.Services
{
	public interface IFeedbackService
	{
		Task<bool> CreateFeedbackAsync(CreateFeedbackDTO createFeedbackDTO, int accountId);
		Task<List<FeedbackDTO>> GetAllFeedbacksAsync();
		Task<List<FeedbackDTO>> GetFeedbacksByAccountIdAsync(int accountId);
	}
}
