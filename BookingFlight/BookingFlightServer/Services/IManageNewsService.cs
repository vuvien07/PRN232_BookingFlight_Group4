using BookingFlightServer.DTO.ManageNews;
using BookingFlightServer.Entities;

namespace BookingFlightServer.Services
{
    public interface IManageNewsService
    {
        Task<List<ResponseNewsDTO>?> GetNewsAsync();
        Task<ResponseNewsDTO?> CreateNewsAsync(RequestAddNewsDTO requestAddNewsDTO);
        Task<bool> DeleteNewsAsync(int newsId);
        Task<bool> UpdateNewsAsync(RequestUpdateNewsDTO requestUpdateNewsDTO);
    }
}
