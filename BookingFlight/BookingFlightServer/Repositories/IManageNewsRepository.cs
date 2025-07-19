using BookingFlightServer.Entities;

namespace BookingFlightServer.Repositories
{
    public interface IManageNewsRepository
    {
        Task<List<News>?> GetNewsAsync();
        Task<News?> CreateNewsAsync(News news);
        Task<bool> DeleteNewsAsync(int newsId);
        Task<bool> UpdateNewsAsync(News news);
    }
}
