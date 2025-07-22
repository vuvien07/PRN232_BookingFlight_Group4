using BookingFlightServer.DTO.Request;
using BookingFlightServer.DTO.Response;
using BookingFlightServer.Entities;

namespace BookingFlightServer.Mappers
{
    public static class FeedbackMapper
    {
        public static FeedbackDTO ToFeedbackResponseDTO(this Feedback feedback)
        {
            if (feedback == null)
                return null;

            return new FeedbackDTO
            {
                FeedbackId = feedback.FeedbackId,
                AccountId = feedback.AccountId,
                Title = feedback.Title,
                Content = feedback.Content,
                Rate = feedback.Rate,
                CreateAt = feedback.CreateAt,
                AccountName = GetAccountName(feedback.Account)
            };
        }

        public static Feedback ToFeedback(this CreateFeedbackDTO feedbackDto, int accountId)
        {
            if (feedbackDto == null)
                return null;

            return new Feedback
            {
                AccountId = accountId,
                Title = feedbackDto.Title,
                Content = feedbackDto.Content,
                Rate = feedbackDto.Rate,
                CreateAt = DateOnly.FromDateTime(DateTime.Now)
            };
        }

        private static string GetAccountName(Account account)
        {
            if (account == null)
                return "Unknown User";

            return account.Username ?? "Unknown User";
        }
    }
}
