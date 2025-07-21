namespace BookingFlightClient.Models.ViewModels
{
    public class ManageNewsEditVM
    {
        public int NewId { get; set; }
        public string Title { get; set; }
        public string? Image { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public int AccountId { get; set; }
    }
}
