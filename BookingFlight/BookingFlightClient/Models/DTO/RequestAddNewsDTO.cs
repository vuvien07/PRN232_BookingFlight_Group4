namespace BookingFlightClient.Models.DTO
{
    public class RequestAddNewsDTO
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public int AccountId { get; set; }
    }
}
