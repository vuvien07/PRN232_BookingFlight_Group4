using BookingFlightServer.DTO;
using BookingFlightServer.Entiies;

namespace BookingFlightServer.Repositories
{
	public interface IFeedbackRepository
	{
		Task<double> getAverageFeedback();

		Task<List<Feedback>?> getFeedbackByFilter(FilterFeedbackDTO filterFeedbackDTO);
	}
}
