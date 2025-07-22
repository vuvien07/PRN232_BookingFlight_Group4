using BookingFlightServer.DTO.Request;
using BookingFlightServer.DTO.Response;
using BookingFlightServer.Entities;
using BookingFlightServer.Repositories;
using BookingFlightServer.Mappers;

namespace BookingFlightServer.Services.Implements
{
	public class FeedbackService : IFeedbackService
	{
		private readonly IFeedbackRepository _feedbackRepository;

		public FeedbackService(IFeedbackRepository feedbackRepository)
		{
			_feedbackRepository = feedbackRepository;
		}

		public async Task<bool> CreateFeedbackAsync(CreateFeedbackDTO createFeedbackDTO, int accountId)
		{
			try
			{
				// Check if customer already has feedback
				var hasExistingFeedback = await _feedbackRepository.HasCustomerAlreadyFeedback(accountId);
				if (hasExistingFeedback)
				{
					// Update existing feedback instead of creating new one
					var existingFeedbacks = await _feedbackRepository.GetFeedbacksByAccountIdAsync(accountId);
					if (existingFeedbacks.Any())
					{
						var existingFeedback = existingFeedbacks.First();
						existingFeedback.Title = createFeedbackDTO.Title;
						existingFeedback.Content = createFeedbackDTO.Content;
						existingFeedback.Rate = createFeedbackDTO.Rate;
						existingFeedback.CreateAt = DateOnly.FromDateTime(DateTime.Now);

						var updatedFeedback = await _feedbackRepository.UpdateAsync(existingFeedback);
						return updatedFeedback != null;
					}
				}

				var feedback = createFeedbackDTO.ToFeedback(accountId);
				var createdFeedback = await _feedbackRepository.CreateAsync(feedback);
				return createdFeedback != null;
			}
			catch
			{
				return false;
			}
		}

		public async Task<List<FeedbackDTO>> GetAllFeedbacksAsync()
		{
			var feedbacks = await _feedbackRepository.GetAllAsync();
			return feedbacks.Select(f => f.ToFeedbackResponseDTO()).ToList();
		}

		public async Task<List<FeedbackDTO>> GetFeedbacksByAccountIdAsync(int accountId)
		{
			var feedbacks = await _feedbackRepository.GetFeedbacksByAccountIdAsync(accountId);
			return feedbacks.Select(f => f.ToFeedbackResponseDTO()).ToList();
		}
	}
}
