using BookingFlightServer.DTO.Request;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
	public class FeedbackRepository : IFeedbackRepository
	{
		private BookingFlightContext _flightContext;

		public FeedbackRepository(BookingFlightContext flightContext)
		{
			_flightContext = flightContext;
		}
		public async Task<double> getAverageFeedback()
		{
			return await _flightContext.Feedbacks.AverageAsync(f => f.Rate);
		}

		public async Task<List<Feedback>?> getFeedbackByFilter(FilterFeedbackDTO filterFeedbackDTO)
		{
			var query = from f in _flightContext.Feedbacks
						join account in _flightContext.Accounts on f.AccountId equals account.AccountId
						join customer in _flightContext.Customers on account.AccountId equals customer.AccountId into customerGroup
						from customer in customerGroup.DefaultIfEmpty()
						select new Feedback()
						{
							FeedbackId = f.FeedbackId,
							AccountId = f.AccountId,
							Content = f.Content,
							Title = f.Title,
							Rate = f.Rate,
							Account = new Account()
							{
								AccountId = account.AccountId,
								Username = account.Username,
								Customer = customer
							}
						};
			if (filterFeedbackDTO.Rate != 0)
			{
				query = query.Where(f => f.Rate == filterFeedbackDTO.Rate);
			}
			query = query.Skip((filterFeedbackDTO.Page - 1) * filterFeedbackDTO.PageSize).Take(filterFeedbackDTO.PageSize);
			return await query.ToListAsync();
		}
	}
}
