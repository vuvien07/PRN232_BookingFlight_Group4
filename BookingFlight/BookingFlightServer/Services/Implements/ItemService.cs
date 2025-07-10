using BookingFlightServer.Data;
using BookingFlightServer.DTO.Manager;
using BookingFlightServer.Services;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Services.Implements
{
    public class ItemService : IItemService
    {
        private readonly BookingFlightContext _context;

        public ItemService(BookingFlightContext context)
        {
            _context = context;
        }

        public async Task<List<ItemDTO>> GetActiveItems()
        {
            return await _context.Items
                .Where(i => i.StatusId == 1) // Active status
                .Include(i => i.Status)
                .Select(i => new ItemDTO
                {
                    ItemId = i.ItemId,
                    ItemName = i.ItemName,
                    Detail = i.Detail,
                    Price = i.Price,
                    StatusId = i.StatusId,
                    StatusName = i.Status != null ? i.Status.StatusType : null,
                    Image = i.Image
                })
                .OrderBy(i => i.ItemName)
                .ToListAsync();
        }

        public async Task<List<ItemDTO>> GetItemsByFilter(ItemListRequestDTO request)
        {
            var query = _context.Items.Include(i => i.Status).AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(i => i.ItemName.Contains(request.SearchTerm) ||
                                        (i.Detail != null && i.Detail.Contains(request.SearchTerm)));
            }

            // Apply status filter
            if (request.StatusId.HasValue)
            {
                query = query.Where(i => i.StatusId == request.StatusId.Value);
            }

            // Apply pagination
            var items = await query
                .Select(i => new ItemDTO
                {
                    ItemId = i.ItemId,
                    ItemName = i.ItemName,
                    Detail = i.Detail,
                    Price = i.Price,
                    StatusId = i.StatusId,
                    StatusName = i.Status != null ? i.Status.StatusType : null,
                    Image = i.Image
                })
                .OrderBy(i => i.ItemName)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return items;
        }

        public async Task<int> GetTotalItemsCount(ItemListRequestDTO request)
        {
            var query = _context.Items.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(i => i.ItemName.Contains(request.SearchTerm) ||
                                        (i.Detail != null && i.Detail.Contains(request.SearchTerm)));
            }

            // Apply status filter
            if (request.StatusId.HasValue)
            {
                query = query.Where(i => i.StatusId == request.StatusId.Value);
            }

            return await query.CountAsync();
        }

        public async Task<ItemDTO?> GetItemById(int itemId)
        {
            return await _context.Items
                .Include(i => i.Status)
                .Where(i => i.ItemId == itemId)
                .Select(i => new ItemDTO
                {
                    ItemId = i.ItemId,
                    ItemName = i.ItemName,
                    Detail = i.Detail,
                    Price = i.Price,
                    StatusId = i.StatusId,
                    StatusName = i.Status != null ? i.Status.StatusType : null,
                    Image = i.Image
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ItemDTO> CreateItem(ItemCreateRequestDTO request)
        {
            Console.WriteLine($"Received item creation request: {System.Text.Json.JsonSerializer.Serialize(request)}");
            
            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.ItemName))
            {
                throw new ArgumentException("Item name is required");
            }
            
            if (request.Price < 0)
            {
                throw new ArgumentException("Price must be non-negative");
            }
            
            var item = new BookingFlightServer.Entities.Item
            {
                ItemName = request.ItemName.Trim(),
                Detail = string.IsNullOrWhiteSpace(request.Detail) ? null : request.Detail.Trim(),
                Price = request.Price,
                StatusId = 1, // Set to active status
                Image = string.IsNullOrWhiteSpace(request.Image) ? "default.jpg" : 
                        (request.Image.Length > 255 ? "default.jpg" : request.Image) // Limit image field size
            };

            Console.WriteLine($"Creating item: {item.ItemName}, Price: {item.Price}, StatusId: {item.StatusId}");

            try
            {
                _context.Items.Add(item);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Item created successfully with ID: {item.ItemId}");

                return await GetItemById(item.ItemId) ?? throw new InvalidOperationException("Failed to retrieve created item");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                throw new InvalidOperationException($"Failed to create item: {ex.Message}", ex);
            }
        }

        public async Task<ItemDTO> UpdateItem(ItemUpdateRequestDTO request)
        {
            var existingItem = await _context.Items.FindAsync(request.ItemId);
            if (existingItem == null)
                throw new InvalidOperationException("Item not found");

            existingItem.ItemName = request.ItemName;
            existingItem.Detail = request.Detail;
            existingItem.Price = request.Price;
            existingItem.StatusId = request.StatusId;
            existingItem.Image = request.Image;

            await _context.SaveChangesAsync();

            return await GetItemById(request.ItemId) ?? throw new InvalidOperationException("Failed to retrieve updated item");
        }

        public async Task<bool> DeleteItem(int itemId)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item == null)
                return false;

            // Soft delete by setting status to inactive
            item.StatusId = 2; // Inactive
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
