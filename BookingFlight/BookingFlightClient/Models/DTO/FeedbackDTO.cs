namespace BookingFlightClient.Models.DTO
{
    public class FeedbackRequestDTO
    {
        public string Title { get; set; } = null!;
        public int Rate { get; set; }
        public string Content { get; set; } = null!;
        public int AccountId { get; set; }
    }

    public class FeedbackResponseDTO
    {
        public int FeedbackId { get; set; }
        public string Title { get; set; } = null!;
        public int Rate { get; set; }
        public string Content { get; set; } = null!;
        public DateOnly? CreateAt { get; set; }
        public int AccountId { get; set; }
        public string AccountUsername { get; set; } = null!;
        public string AccountFullName { get; set; } = null!;
    }

    public class FilterFeedbackDTO
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Rate { get; set; }
        public long TotalFeedback { get; set; }
        public long TotalPage { get; set; }
    }

    public class FeedbackStatisticsDTO
    {
        public int TotalFeedbacks { get; set; }
        public double AverageRating { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new();
        public List<FeedbackResponseDTO> RecentFeedbacks { get; set; } = new();
    }

    public class UpdateFeedbackRequest
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
