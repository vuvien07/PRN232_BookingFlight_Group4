using BookingFlightServer.DTO.Manager;

namespace BookingFlightServer.Services
{
    public interface IItemService
    {
        Task<List<ItemDTO>> GetActiveItems();
        Task<List<ItemDTO>> GetItemsByFilter(ItemListRequestDTO request);
        Task<int> GetTotalItemsCount(ItemListRequestDTO request);
        Task<ItemDTO?> GetItemById(int itemId);
        Task<ItemDTO> CreateItem(ItemCreateRequestDTO request);
        Task<ItemDTO> UpdateItem(ItemUpdateRequestDTO request);
        Task<bool> DeleteItem(int itemId);
    }
}
