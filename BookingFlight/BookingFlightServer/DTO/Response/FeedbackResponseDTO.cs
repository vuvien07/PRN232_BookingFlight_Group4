namespace BookingFlightServer.DTO.Response
{
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
}