using Microsoft.AspNetCore.Mvc;
using BookingFlightClient.Services;
using BookingFlightClient.Models.DTO;

namespace BookingFlightClient.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 9, string? searchTerm = null, string? category = null)
        {
            try
            {
                var newsData = await _newsService.GetNewsAsync(page, pageSize, searchTerm, category);
                return View(newsData);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error loading news: {ex.Message}");
                
                // Return empty model on error
                return View(new NewsIndexDto
                {
                    News = new List<NewsDto>(),
                    CurrentPage = 1,
                    TotalPages = 0,
                    PageSize = pageSize,
                    TotalNews = 0
                });
            }
        }
        
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var news = await _newsService.GetNewsByIdAsync(id);
                if (news == null)
                {
                    return NotFound();
                }
                
                return View(news);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading news details: {ex.Message}");
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            try
            {
                var categories = await _newsService.GetCategoriesAsync();
                return Json(categories);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading categories: {ex.Message}");
                return Json(new List<string>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Related(int id, string category, int count = 3)
        {
            try
            {
                var relatedNews = await _newsService.GetRelatedNewsAsync(id, category, count);
                return Json(relatedNews);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading related news: {ex.Message}");
                return Json(new List<NewsDto>());
            }
        }
    }
}
