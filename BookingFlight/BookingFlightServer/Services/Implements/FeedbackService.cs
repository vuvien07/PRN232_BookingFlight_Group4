using BookingFlightServer.Data;
using BookingFlightServer.DTO.Request;
using BookingFlightServer.DTO.Response;
using BookingFlightServer.Entities;
using BookingFlightServer.Mappers;
using BookingFlightServer.Repositories;
using BookingFlightServer.Services;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Services.Implements
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IFeedbackTicketMappingService _mappingService;
        private readonly ILogger<FeedbackService> _logger;

        public FeedbackService(IFeedbackRepository feedbackRepository, IFeedbackTicketMappingService mappingService, ILogger<FeedbackService> logger)
        {
            _feedbackRepository = feedbackRepository;
            _mappingService = mappingService;
            _logger = logger;
        }

        public async Task<bool> CreateFeedbackAsync(CreateFeedbackDTO createFeedbackDTO, int accountId)
        {
            try
            {
                _logger.LogInformation("Creating feedback for account {AccountId} for ticket {TicketId}", accountId, createFeedbackDTO.TicketId);
                
                // Check if user already has feedback for this ticket
                if (createFeedbackDTO.TicketId.HasValue)
                {
                    var existingFeedback = _mappingService.HasFeedbackForTicketAndAccount(createFeedbackDTO.TicketId.Value, accountId);
                    if (existingFeedback)
                    {
                        throw new InvalidOperationException("Bạn đã gửi feedback cho chuyến bay này rồi!");
                    }
                }
                
                // Create new feedback
                var feedback = createFeedbackDTO.ToFeedback(accountId);
                var createdFeedback = await _feedbackRepository.CreateAsync(feedback);
                
                // Create mapping between feedback and ticket
                if (createFeedbackDTO.TicketId.HasValue)
                {
                    _mappingService.LinkFeedbackToTicketAndAccount(createdFeedback.FeedbackId, createFeedbackDTO.TicketId.Value, accountId);
                    _logger.LogInformation("Created mapping: FeedbackId {FeedbackId} -> TicketId {TicketId} for AccountId {AccountId}", 
                        createdFeedback.FeedbackId, createFeedbackDTO.TicketId.Value, accountId);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating feedback");
                throw;
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

        public async Task<List<FeedbackDTO>> GetFeedbacksByTicketIdAsync(int ticketId)
        {
            var feedbacks = await _feedbackRepository.GetFeedbacksByTicketIdAsync(ticketId);
            return feedbacks.Select(f => f.ToFeedbackResponseDTO()).ToList();
        }

        public async Task<List<FeedbackDetailDTO>> GetFeedbacksDetailByTicketIdAsync(int ticketId)
        {
            return await _feedbackRepository.GetFeedbacksDetailByTicketIdAsync(ticketId);
        }

        public async Task<List<FeedbackDetailDTO>> GetFeedbacksDetailByAccountIdAsync(int accountId)
        {
            return await _feedbackRepository.GetFeedbacksDetailByAccountIdAsync(accountId);
        }

        public async Task<List<FeedbackDetailDTO>> GetAllFeedbackDetailsAsync()
        {
            return await _feedbackRepository.GetAllFeedbackDetailsAsync();
        }

        public async Task<List<FeedbackDetailDTO>> SearchAndFilterFeedbacksAsync(string? searchTerm = null, int? rating = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            return await _feedbackRepository.SearchAndFilterFeedbacksAsync(searchTerm, rating, fromDate, toDate);
        }

        public async Task<bool> HasFeedbackByTicketIdAsync(int ticketId, int accountId)
        {
            try
            {
                Console.WriteLine($"[DEBUG] HasFeedbackByTicketIdAsync called with TicketId: {ticketId}, AccountId: {accountId}");
                
                // Get all feedbacks by account and check for ticket ID in content
                var feedbacks = await _feedbackRepository.GetFeedbacksByAccountIdAsync(accountId);
                bool hasFeedback = feedbacks.Any(f => f.Content?.Contains($"[TICKET:{ticketId}]") == true);
                
                Console.WriteLine($"[DEBUG] HasFeedbackByTicketIdAsync returning: {hasFeedback}");
                return hasFeedback;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Exception in HasFeedbackByTicketIdAsync: {ex.Message}");
                _logger.LogError(ex, "Error checking feedback by ticket ID");
                return false;
            }
        }

        public async Task<List<FeedbackDetailDTO>> GetFeedbackDetailsByTicketIdAsync(int ticketId, int accountId)
        {
            try
            {
                // Get all feedbacks by account and filter by ticket ID in content
                var feedbacks = await _feedbackRepository.GetFeedbacksByAccountIdAsync(accountId);
                var ticketFeedback = feedbacks.Where(f => f.Content?.Contains($"[TICKET:{ticketId}]") == true).FirstOrDefault();
                
                if (ticketFeedback != null)
                {
                    // Create a mock FeedbackDetailDTO since we don't have full join data
                    var detail = new FeedbackDetailDTO
                    {
                        FeedbackId = ticketFeedback.FeedbackId,
                        Title = ticketFeedback.Title,
                        Rate = ticketFeedback.Rate,
                        Content = ticketFeedback.Content?.Replace($"[TICKET:{ticketId}]", ""), // Remove ticket tag from display
                        CreateAt = ticketFeedback.CreateAt,
                        AccountId = ticketFeedback.AccountId,
                        AccountName = ticketFeedback.Account?.Username ?? "Unknown",
                        TicketId = ticketId
                    };
                    return new List<FeedbackDetailDTO> { detail };
                }
                
                return new List<FeedbackDetailDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feedback details by ticket ID and account ID");
                return new List<FeedbackDetailDTO>();
            }
        }

        public async Task<List<FeedbackDetailDTO>> GetFeedbackDetailsByTicketIdAsync(int ticketId)
        {
            try
            {
                var allFeedbacks = await _feedbackRepository.GetFeedbacksDetailByTicketIdAsync(ticketId);
                return allFeedbacks.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feedback details by ticket ID");
                return new List<FeedbackDetailDTO>();
            }
        }
    }
}