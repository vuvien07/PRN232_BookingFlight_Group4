using BookingFlightServer.Data;
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

		public async Task<Feedback> CreateAsync(Feedback feedback)
		{
			_flightContext.Feedbacks.Add(feedback);
			await _flightContext.SaveChangesAsync();
			return feedback;
		}

		public async Task<List<Feedback>> GetAllAsync()
		{
			return await _flightContext.Feedbacks
				.Include(f => f.Account)
				.OrderByDescending(f => f.CreateAt)
				.ToListAsync();
		}

		public async Task<List<Feedback>> GetFeedbacksByAccountIdAsync(int accountId)
		{
			return await _flightContext.Feedbacks
				.Include(f => f.Account)
				.Where(f => f.AccountId == accountId)
				.OrderByDescending(f => f.CreateAt)
				.ToListAsync();
		}

		public async Task<Feedback?> GetByIdAsync(int id)
		{
			return await _flightContext.Feedbacks
				.Include(f => f.Account)
				.FirstOrDefaultAsync(f => f.FeedbackId == id);
		}

		public async Task<bool> HasCustomerAlreadyFeedback(int accountId)
		{
			return await _flightContext.Feedbacks
				.AnyAsync(f => f.AccountId == accountId);
		}

		public async Task<Feedback> UpdateAsync(Feedback feedback)
		{
			_flightContext.Feedbacks.Update(feedback);
			await _flightContext.SaveChangesAsync();
			return feedback;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var feedback = await _flightContext.Feedbacks.FindAsync(id);
			if (feedback == null)
				return false;

			_flightContext.Feedbacks.Remove(feedback);
			await _flightContext.SaveChangesAsync();
			return true;
		}
	}
}
