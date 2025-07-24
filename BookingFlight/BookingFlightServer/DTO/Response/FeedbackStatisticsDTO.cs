namespace BookingFlightServer.DTO.Response
{
    public class FeedbackStatisticsDTO
    {
        public int TotalFeedbacks { get; set; }
        public double AverageRating { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new();
        public List<FeedbackResponseDTO> RecentFeedbacks { get; set; } = new();
    }
}
