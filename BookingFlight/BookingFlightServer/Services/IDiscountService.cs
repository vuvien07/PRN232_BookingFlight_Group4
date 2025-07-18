using BookingFlightServer.DTO.Manager;

namespace BookingFlightServer.Services
{
    public interface IDiscountService
    {
        Task<List<DiscountDTO>> GetDiscountsByFilter(DiscountListRequestDTO request);
        Task<int> GetTotalDiscountsCount(DiscountListRequestDTO request);
        Task<DiscountDTO?> GetDiscountById(int id);
        Task<DiscountDTO> CreateDiscount(DiscountCreateRequestDTO request);
        Task<DiscountDTO> UpdateDiscount(DiscountUpdateRequestDTO request);
        Task<bool> DeleteDiscount(int id);
        Task<List<object>> GetDiscountStatuses();
    }
}
