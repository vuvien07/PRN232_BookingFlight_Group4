using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
    public class ManageNewsRepository : IManageNewsRepository
    {
        private readonly BookingFlightContext bookingFlightContext;

        public ManageNewsRepository(BookingFlightContext bookingFlightContext)
        {
            this.bookingFlightContext = bookingFlightContext;
        }
        public async Task<News?> CreateNewsAsync(News news)
        {
            bookingFlightContext.News.Add(news);
            await bookingFlightContext.SaveChangesAsync();
            // Return the created news item or null if creation failed
            return news == null ? null : news;
        }

        public async Task<bool> DeleteNewsAsync(int newsId)
        {
            // Find the news item by its ID
            var news = await bookingFlightContext.News.FirstOrDefaultAsync(n => n.NewId == newsId);
            // Check if the news entity exists
            if (news == null)
            {
                return false;
            }
            // Remove the news entity from the context
            bookingFlightContext.News.Remove(news);
            await bookingFlightContext.SaveChangesAsync();
            // Return true to indicate successful deletion
            return true;
        }

        public async Task<List<News>?> GetNewsAsync()
        {
            // Retrieve the list of news items, including related Account entities
            var newsList = await bookingFlightContext.News.Include(n => n.Account).ToListAsync();
            // Check if the news list is null or empty
            return newsList == null ? null : newsList;
        }

        public async Task<bool> UpdateNewsAsync(News news)
        {
            // Find the existing news item by its ID
            var existingNews = await bookingFlightContext.News.FirstOrDefaultAsync(n => n.NewId == news.NewId);
            // Check if the news entity exists
            if (existingNews == null)
            {
                return false;
            }
            // Update the properties of the existing news entity
            existingNews.Title = news.Title;
            existingNews.Content = news.Content;
            existingNews.Image = news.Image;
            existingNews.Category = news.Category;
            existingNews.Author = news.Author;
            existingNews.AccountId = news.AccountId;
            // Update the existing news entity
            bookingFlightContext.News.Update(existingNews);
            await bookingFlightContext.SaveChangesAsync();
            // Return true to indicate successful update
            return true;

        }
    }
}
