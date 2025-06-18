using BookingFlightServer.DTO.Request;
using BookingFlightServer.Entities;
namespace BookingFlightServer.Repositories
{
	public interface IFeedbackRepository
	{
		Task<double> getAverageFeedback();

		Task<List<Feedback>?> getFeedbackByFilter(FilterFeedbackDTO filterFeedbackDTO);
	}
}
