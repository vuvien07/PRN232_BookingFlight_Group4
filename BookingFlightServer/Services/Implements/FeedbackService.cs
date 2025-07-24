using BookingFlightServer.Data;
using BookingFlightServer.DTO;
using BookingFlightServer.Entities;
using BookingFlightServer.Mappers;
using BookingFlightServer.Repositories;
using BookingFlightServer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Services.Implements
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ILogger<FeedbackService> _logger;

        public FeedbackService(IFeedbackRepository feedbackRepository, ILogger<FeedbackService> logger)
        {
            _feedbackRepository = feedbackRepository;
            _logger = logger;
        }

        public async Task<FeedbackDTO> CreateFeedbackAsync(int accountId, CreateFeedbackDTO createFeedbackDto)
        {
            _logger.LogInformation($"Creating feedback for AccountId: {accountId}");

            // Check if user already has feedback
            var existingFeedback = await _feedbackRepository.GetFeedbackByAccountIdAsync(accountId);
            
            Feedback feedback;
            
            if (existingFeedback != null)
            {
                _logger.LogInformation($"Updating existing feedback for AccountId: {accountId}");
                
                // Update existing feedback
                existingFeedback.Rating = createFeedbackDto.Rating;
                existingFeedback.Comment = createFeedbackDto.Comment;
                existingFeedback.CreateAt = DateOnly.FromDateTime(DateTime.Now);
                
                feedback = await _feedbackRepository.UpdateFeedbackAsync(existingFeedback);
            }
            else
            {
                _logger.LogInformation($"Creating new feedback for AccountId: {accountId}");
                
                // Create new feedback
                feedback = FeedbackMapper.ToEntity(createFeedbackDto, accountId);
                feedback = await _feedbackRepository.CreateFeedbackAsync(feedback);
            }

            return FeedbackMapper.ToDTO(feedback);
        }

        public async Task<IEnumerable<FeedbackDTO>> GetAllFeedbacksAsync()
        {
            var feedbacks = await _feedbackRepository.GetAllFeedbacksAsync();
            return feedbacks.Select(f => FeedbackMapper.ToDTO(f));
        }

        public async Task<FeedbackDTO?> GetFeedbackByAccountIdAsync(int accountId)
        {
            var feedback = await _feedbackRepository.GetFeedbackByAccountIdAsync(accountId);
            return feedback != null ? FeedbackMapper.ToDTO(feedback) : null;
        }

        public async Task<IEnumerable<FeedbackDetailDTO>> GetFeedbacksByTicketIdAsync(int ticketId)
        {
            return await _feedbackRepository.GetFeedbacksByTicketIdAsync(ticketId);
        }

        public async Task<IEnumerable<FeedbackDetailDTO>> GetFeedbacksByFlightIdAsync(int flightId)
        {
            return await _feedbackRepository.GetFeedbacksByFlightIdAsync(flightId);
        }

        public async Task<IEnumerable<FeedbackDetailDTO>> GetMyFeedbackDetailsAsync(int accountId)
        {
            return await _feedbackRepository.GetMyFeedbackDetailsAsync(accountId);
        }
    }
}
