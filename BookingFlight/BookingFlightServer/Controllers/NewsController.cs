using BookingFlightServer.Entities;
using BookingFlightServer.Services;
using BookingFlightServer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly BookingFlightContext _context;

        public NewsController(BookingFlightContext context)
        {
            _context = context;
        }

        // GET: api/news
        [HttpGet]
        public async Task<IActionResult> GetNews(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 9, 
            [FromQuery] string? searchTerm = null, 
            [FromQuery] string? category = null)
        {
            try
            {
                var query = _context.News.Include(n => n.Account).AsQueryable();

                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(n => 
                        n.Title.Contains(searchTerm) || 
                        n.Content.Contains(searchTerm) ||
                        n.Category.Contains(searchTerm));
                }

                // Apply category filter
                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(n => n.Category == category);
                }

                // Get total count for pagination
                var totalNews = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalNews / (double)pageSize);

                // Get news for current page
                var news = await query
                    .OrderByDescending(n => n.NewId) // Assuming NewId is created in descending order
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(n => new
                    {
                        NewId = n.NewId,
                        Title = n.Title,
                        Image = n.Image,
                        Content = n.Content,
                        Category = n.Category,
                        Author = n.Author,
                        PublishedDate = DateTime.Now, // Since there's no date field in entity
                        Summary = n.Content.Length > 200 ? n.Content.Substring(0, 200) + "..." : n.Content,
                        ViewCount = new Random().Next(100, 2000), // Random view count since not in entity
                        IsActive = true,
                        ImageUrl = !string.IsNullOrEmpty(n.Image) ? n.Image : "https://images.unsplash.com/photo-1586953208448-b95a79798f07?w=800&h=400&fit=crop"
                    })
                    .ToListAsync();

                var result = new
                {
                    News = news,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    TotalNews = totalNews,
                    SearchTerm = searchTerm,
                    CategoryFilter = category,
                    HasPreviousPage = page > 1,
                    HasNextPage = page < totalPages
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/news/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewsById(int id)
        {
            try
            {
                var news = await _context.News
                    .Include(n => n.Account)
                    .Where(n => n.NewId == id)
                    .Select(n => new
                    {
                        NewId = n.NewId,
                        Title = n.Title,
                        Image = n.Image,
                        Content = n.Content,
                        Category = n.Category,
                        Author = n.Author,
                        PublishedDate = DateTime.Now,
                        Summary = n.Content.Length > 200 ? n.Content.Substring(0, 200) + "..." : n.Content,
                        ViewCount = new Random().Next(100, 2000),
                        IsActive = true,
                        ImageUrl = !string.IsNullOrEmpty(n.Image) ? n.Image : "https://images.unsplash.com/photo-1586953208448-b95a79798f07?w=800&h=400&fit=crop"
                    })
                    .FirstOrDefaultAsync();

                if (news == null)
                {
                    return NotFound(new { message = "News not found" });
                }

                return Ok(news);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/news/related/{id}
        [HttpGet("related/{id}")]
        public async Task<IActionResult> GetRelatedNews(int id, [FromQuery] string? category = null, [FromQuery] int count = 3)
        {
            try
            {
                var query = _context.News.Include(n => n.Account).Where(n => n.NewId != id);

                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(n => n.Category == category);
                }

                var relatedNews = await query
                    .Take(count)
                    .Select(n => new
                    {
                        NewId = n.NewId,
                        Title = n.Title,
                        Image = n.Image,
                        Content = n.Content,
                        Category = n.Category,
                        Author = n.Author,
                        PublishedDate = DateTime.Now,
                        Summary = n.Content.Length > 200 ? n.Content.Substring(0, 200) + "..." : n.Content,
                        ViewCount = new Random().Next(100, 2000),
                        IsActive = true,
                        ImageUrl = !string.IsNullOrEmpty(n.Image) ? n.Image : "https://images.unsplash.com/photo-1586953208448-b95a79798f07?w=800&h=400&fit=crop"
                    })
                    .ToListAsync();

                return Ok(relatedNews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/news/categories
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _context.News
                    .Select(n => n.Category)
                    .Distinct()
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
