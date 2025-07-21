namespace BookingFlightClient.Models.ViewModels
{
    public class NewsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Summary { get; set; } = "";
        public string Content { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public DateTime PublishedDate { get; set; }
        public string Author { get; set; } = "";
        public string Category { get; set; } = "";
    }
    
    public class NewsIndexViewModel
    {
        public List<NewsViewModel> News { get; set; } = new List<NewsViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalNews { get; set; }
        
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
