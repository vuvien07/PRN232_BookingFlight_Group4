using BookingFlightServer.DTO.ManageNews;
using BookingFlightServer.Entities;
using BookingFlightServer.Repositories;

namespace BookingFlightServer.Services.Implements
{
    public class ManageNewsService : IManageNewsService
    {
        private readonly IManageNewsRepository manageNewsRepository;

        public ManageNewsService(IManageNewsRepository manageNewsRepository)
        {
            this.manageNewsRepository = manageNewsRepository;
        }
        public async Task<ResponseNewsDTO?> CreateNewsAsync(RequestAddNewsDTO requestAddNewsDTO)
        {
            var news = new News
            {
                NewId = requestAddNewsDTO.NewId,
                Title = requestAddNewsDTO.Title,
                Image = requestAddNewsDTO.Image,
                Content = requestAddNewsDTO.Content,
                Category = requestAddNewsDTO.Category,
                Author = requestAddNewsDTO.Author,
                AccountId = requestAddNewsDTO.AccountId
            };

            var createdNews = await manageNewsRepository.CreateNewsAsync(news);

            if (createdNews == null)
            {
                return null;
            }

            return new ResponseNewsDTO
            {
                NewId = createdNews.NewId,
                Title = createdNews.Title,
                Image = createdNews.Image,
                Content = createdNews.Content,
                Category = createdNews.Category,
                Author = createdNews.Author,
                AccountId = createdNews.AccountId,
            };

        }

        public async Task<bool> DeleteNewsAsync(int newsId)
        {
            // Validate the newsId
            if (newsId <= 0)
            {
                return false;
            }
            // Call the repository to delete the news
            var isDeleted = await manageNewsRepository.DeleteNewsAsync(newsId);
            return isDeleted;
        }

        public async Task<List<ResponseNewsDTO>?> GetNewsAsync()
        {
            // Call the repository to get the list of news
            var newsList = await manageNewsRepository.GetNewsAsync();

            // If the list is null, return null
            if (newsList == null) return null;

            // Map the list of News entities to a list of ResponseNewsDTO
            var responseNewsDTOList = newsList.Select(news => new ResponseNewsDTO
            {
                NewId = news.NewId,
                Title = news.Title,
                Image = news.Image,
                Content = news.Content,
                Category = news.Category,
                Author = news.Author,
                AccountId = news.AccountId,
                AccountName = news.Account?.Username ?? "Unknown"
            }).ToList();

            // Return the list of ResponseNewsDTO
            return responseNewsDTOList;
        }

        public async Task<ResponseNewsDTO?> GetNewsByIdAsync(int newsId)
        {
            var news = await manageNewsRepository.GetNewsByIdAsync(newsId);

            // If the news is null, return null
            if (news == null) return null;

            // Map the News entity to ResponseNewsDTO
            return new ResponseNewsDTO
            {
                NewId = news.NewId,
                Title = news.Title,
                Image = news.Image,
                Content = news.Content,
                Category = news.Category,
                Author = news.Author,
                AccountId = news.AccountId,
                AccountName = news.Account?.Username ?? "Unknown"
            };
        }

        public async Task<bool> UpdateNewsAsync(RequestUpdateNewsDTO requestUpdateNewsDTO)
        {
            // Validate the request
            if (requestUpdateNewsDTO == null || requestUpdateNewsDTO.NewId <= 0)
            {
                return false;
            }

            var isUpdated = await manageNewsRepository.UpdateNewsAsync(new News
            {
                NewId = requestUpdateNewsDTO.NewId,
                Title = requestUpdateNewsDTO.Title,
                Image = requestUpdateNewsDTO.Image,
                Content = requestUpdateNewsDTO.Content,
                Category = requestUpdateNewsDTO.Category,
                Author = requestUpdateNewsDTO.Author,
            });

            return isUpdated;
        }
    }
}
