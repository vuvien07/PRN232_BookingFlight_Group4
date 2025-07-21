using BookingFlightClient.Models.DTO;
using System.Text.Json;

namespace BookingFlightClient.Services
{
    public interface INewsService
    {
        Task<NewsIndexDto> GetNewsAsync(int page = 1, int pageSize = 9, string? searchTerm = null, string? category = null);
        Task<NewsDto?> GetNewsByIdAsync(int id);
        Task<List<NewsDto>> GetRelatedNewsAsync(int currentNewsId, string category, int count = 3);
        Task<List<string>> GetCategoriesAsync();
    }

    public class NewsService : INewsService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public NewsService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5077/api";
        }

        public async Task<NewsIndexDto> GetNewsAsync(int page = 1, int pageSize = 9, string? searchTerm = null, string? category = null)
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"page={page}",
                    $"pageSize={pageSize}"
                };

                if (!string.IsNullOrEmpty(searchTerm))
                    queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");

                if (!string.IsNullOrEmpty(category))
                    queryParams.Add($"category={Uri.EscapeDataString(category)}");

                var query = string.Join("&", queryParams);
                var response = await _httpClient.GetAsync($"{_baseUrl}/news?{query}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<NewsIndexDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return result ?? new NewsIndexDto();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching news: {ex.Message}");
            }

            // Fallback to sample data if API fails
            return GetSampleNewsData(page, pageSize, searchTerm, category);
        }

        public async Task<NewsDto?> GetNewsByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/news/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<NewsDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching news by id: {ex.Message}");
            }

            // Fallback to sample data
            return GetSampleNewsData(1, 100).News.FirstOrDefault(n => n.NewId == id);
        }

        public async Task<List<NewsDto>> GetRelatedNewsAsync(int currentNewsId, string category, int count = 3)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/news/related/{currentNewsId}?category={Uri.EscapeDataString(category)}&count={count}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<List<NewsDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return result ?? new List<NewsDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching related news: {ex.Message}");
            }

            // Fallback to sample data
            var sampleData = GetSampleNewsData(1, 100);
            return sampleData.News
                .Where(n => n.NewId != currentNewsId && n.Category == category)
                .Take(count)
                .ToList();
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/news/categories");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<List<string>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return result ?? new List<string>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching categories: {ex.Message}");
            }

            // Fallback categories
            return new List<string> { "Đường bay mới", "Khuyến mãi", "Công nghệ", "Quy định", "Tin tức", "Đầu tư", "Chương trình" };
        }

        private NewsIndexDto GetSampleNewsData(int page = 1, int pageSize = 9, string? searchTerm = null, string? category = null)
        {
            var allNews = new List<NewsDto>
            {
                new NewsDto
                {
                    NewId = 1,
                    Title = "Vietnam Airlines mở đường bay mới Hà Nội - Tokyo",
                    Summary = "Vietnam Airlines chính thức khai trương đường bay thẳng từ Hà Nội đến Tokyo, tăng cường kết nối du lịch và thương mại giữa Việt Nam và Nhật Bản.",
                    Content = "Vietnam Airlines đã chính thức khai trương đường bay thẳng từ Hà Nội đến Tokyo vào ngày 15/12/2024. Đường bay mới này sẽ được khai thác với tần suất 4 chuyến/tuần bằng máy bay Boeing 787-9 Dreamliner, mang đến trải nghiệm bay hiện đại và thoải mái cho hành khách.\n\nThời gian bay dự kiến là 3 giờ 30 phút, giúp rút ngắn đáng kể thời gian di chuyển giữa hai thủ đô. Đây là bước tiến quan trọng trong việc tăng cường hợp tác kinh tế và du lịch giữa Việt Nam và Nhật Bản.\n\nGiá vé khởi điểm từ 12 triệu đồng cho hạng phổ thông và 35 triệu đồng cho hạng thương gia. Hành khách có thể đặt vé từ hôm nay thông qua website chính thức và các đại lý ủy quyền.",
                    Image = "https://images.unsplash.com/photo-1436491865332-7a61a109cc05?w=800&h=400&fit=crop",
                    PublishedDate = DateTime.Now.AddDays(-2),
                    Author = "Nguyễn Văn A",
                    Category = "Đường bay mới",
                    IsActive = true,
                    ViewCount = 1250
                },
                new NewsDto
                {
                    NewId = 2,
                    Title = "Jetstar Pacific giảm giá vé máy bay đến 50% trong tháng 1",
                    Summary = "Chương trình khuyến mãi lớn của Jetstar Pacific với mức giảm giá lên đến 50% cho các chuyến bay nội địa trong tháng 1/2025.",
                    Content = "Nhằm kích cầu du lịch đầu năm, Jetstar Pacific đã tung ra chương trình khuyến mãi 'Bay sang năm mới' với mức giảm giá lên đến 50% cho tất cả các đường bay nội địa.\n\nChương trình áp dụng từ ngày 1/1/2025 đến 31/1/2025 cho các chuyến bay từ ngày 15/1 đến 28/2/2025. Đặc biệt, khách hàng đặt vé sớm sẽ được ưu đãi thêm 10% và miễn phí đổi lịch bay một lần.\n\nCác tuyến đường được áp dụng khuyến mãi bao gồm: Hà Nội - TP.HCM, Đà Nẵng - TP.HCM, Hà Nội - Đà Lạt, và nhiều tuyến khác. Hành khách có thể đặt vé qua website, ứng dụng di động hoặc tại các văn phòng bán vé.",
                    Image = "https://images.unsplash.com/photo-1544620347-c4fd4a3d5957?w=800&h=400&fit=crop",
                    PublishedDate = DateTime.Now.AddDays(-1),
                    Author = "Trần Thị B",
                    Category = "Khuyến mãi",
                    IsActive = true,
                    ViewCount = 890
                },
                new NewsDto
                {
                    NewId = 3,
                    Title = "Sân bay Tân Sơn Nhất nâng cấp hệ thống check-in tự động",
                    Summary = "Sân bay Tân Sơn Nhất đầu tư nâng cấp hệ thống check-in tự động mới, giúp giảm thời gian chờ đợi cho hành khách.",
                    Content = "Cảng hàng không quốc tế Tân Sơn Nhất vừa hoàn thành việc nâng cấp hệ thống check-in tự động với 50 kiosk mới được lắp đặt tại cả hai nhà ga quốc nội và quốc tế.\n\nHệ thống mới hỗ trợ check-in cho tất cả các hãng hàng không, giúp rút ngắn thời gian làm thủ tục từ 30-40 phút xuống còn 10-15 phút. Đặc biệt, hệ thống tích hợp công nghệ nhận diện khuôn mặt và quét QR code, mang lại trải nghiệm hiện đại cho hành khách.\n\nBên cạnh đó, sân bay cũng triển khai ứng dụng di động mới cho phép hành khách theo dõi thời gian chờ đợi tại các quầy check-in và an ninh theo thời gian thực.",
                    Image = "https://images.unsplash.com/photo-1581833971358-2c8b550f87b3?w=800&h=400&fit=crop",
                    PublishedDate = DateTime.Now.AddDays(-3),
                    Author = "Lê Văn C",
                    Category = "Công nghệ",
                    IsActive = true,
                    ViewCount = 670
                }
            };

            // Add more sample data...
            for (int i = 4; i <= 50; i++)
            {
                allNews.Add(new NewsDto
                {
                    NewId = i,
                    Title = $"Tin tức hàng không số {i}",
                    Summary = $"Tóm tắt tin tức số {i} về ngành hàng không và du lịch.",
                    Content = $"Nội dung chi tiết của tin tức số {i}...",
                    Image = $"https://images.unsplash.com/photo-{1500000000000 + i}?w=800&h=400&fit=crop",
                    PublishedDate = DateTime.Now.AddDays(-i),
                    Author = $"Tác giả {i}",
                    Category = new[] { "Tin tức", "Công nghệ", "Khuyến mãi", "Đường bay mới" }[i % 4],
                    IsActive = true,
                    ViewCount = 100 + (i * 10)
                });
            }

            // Apply filters
            var filteredNews = allNews.Where(n => n.IsActive);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                filteredNews = filteredNews.Where(n =>
                    n.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    n.Summary.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    n.Category.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(category))
            {
                filteredNews = filteredNews.Where(n => n.Category == category);
            }

            var totalNews = filteredNews.Count();
            var totalPages = (int)Math.Ceiling(totalNews / (double)pageSize);

            var newsForPage = filteredNews
                .OrderByDescending(n => n.PublishedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new NewsIndexDto
            {
                News = newsForPage,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalNews = totalNews,
                SearchTerm = searchTerm,
                CategoryFilter = category
            };
        }
    }
}
