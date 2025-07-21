namespace BookingFlightClient.Models.DTO
{
    public class NewsDto
    {
        public int NewId { get; set; }
        public string Title { get; set; } = "";
        public string? Image { get; set; }
        public string Content { get; set; } = "";
        public string Category { get; set; } = "";
        public string Author { get; set; } = "";
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        public string Summary { get; set; } = "";
        public int ViewCount { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Additional properties for display
        public string ImageUrl => !string.IsNullOrEmpty(Image) ? Image : "https://images.unsplash.com/photo-1586953208448-b95a79798f07?w=800&h=400&fit=crop";
    }
    
    public class NewsIndexDto
    {
        public List<NewsDto> News { get; set; } = new List<NewsDto>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalNews { get; set; }
        public string? SearchTerm { get; set; }
        public string? CategoryFilter { get; set; }
        
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
