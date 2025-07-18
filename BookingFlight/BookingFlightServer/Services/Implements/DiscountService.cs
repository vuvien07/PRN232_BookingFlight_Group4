using BookingFlightServer.Data;
using BookingFlightServer.DTO.Manager;
using BookingFlightServer.Services;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Services.Implements
{
    public class DiscountService : IDiscountService
    {
        private readonly BookingFlightContext _context;

        public DiscountService(BookingFlightContext context)
        {
            _context = context;
        }

        public async Task<List<DiscountDTO>> GetDiscountsByFilter(DiscountListRequestDTO request)
        {
            var query = _context.Discounts
                .Include(d => d.Customer)
                    .ThenInclude(c => c.Account)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(d => 
                    d.DiscountCode.Contains(request.SearchTerm) ||
                    d.DiscountTitle.Contains(request.SearchTerm) ||
                    (d.Customer != null && d.Customer.Account != null && d.Customer.Account.Username.Contains(request.SearchTerm)));
            }

            if (request.Status.HasValue)
            {
                query = query.Where(d => d.Status == request.Status.Value);
            }

            if (request.CustomerId.HasValue)
            {
                query = query.Where(d => d.CustomerId == request.CustomerId.Value);
            }

            // Apply pagination
            var discounts = await query
                .OrderByDescending(d => d.DiscountId)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(d => new DiscountDTO
                {
                    DiscountId = d.DiscountId,
                    DiscountCode = d.DiscountCode,
                    DiscountPercent = d.DiscountPercent,
                    DiscountTitle = d.DiscountTitle,
                    DiscountInfor = d.DiscountInfor,
                    CustomerId = d.CustomerId,
                    CustomerName = d.Customer != null && d.Customer.Account != null ? d.Customer.Account.Username : null,
                    Status = d.Status
                })
                .ToListAsync();

            return discounts;
        }

        public async Task<int> GetTotalDiscountsCount(DiscountListRequestDTO request)
        {
            var query = _context.Discounts
                .Include(d => d.Customer)
                    .ThenInclude(c => c.Account)
                .AsQueryable();

            // Apply same filters as GetDiscountsByFilter
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(d => 
                    d.DiscountCode.Contains(request.SearchTerm) ||
                    d.DiscountTitle.Contains(request.SearchTerm) ||
                    (d.Customer != null && d.Customer.Account != null && d.Customer.Account.Username.Contains(request.SearchTerm)));
            }

            if (request.Status.HasValue)
            {
                query = query.Where(d => d.Status == request.Status.Value);
            }

            if (request.CustomerId.HasValue)
            {
                query = query.Where(d => d.CustomerId == request.CustomerId.Value);
            }

            return await query.CountAsync();
        }

        public async Task<DiscountDTO?> GetDiscountById(int id)
        {
            var discount = await _context.Discounts
                .Include(d => d.Customer)
                    .ThenInclude(c => c.Account)
                .Where(d => d.DiscountId == id)
                .Select(d => new DiscountDTO
                {
                    DiscountId = d.DiscountId,
                    DiscountCode = d.DiscountCode,
                    DiscountPercent = d.DiscountPercent,
                    DiscountTitle = d.DiscountTitle,
                    DiscountInfor = d.DiscountInfor,
                    CustomerId = d.CustomerId,
                    CustomerName = d.Customer != null && d.Customer.Account != null ? d.Customer.Account.Username : null,
                    Status = d.Status
                })
                .FirstOrDefaultAsync();

            return discount;
        }

        public async Task<DiscountDTO> CreateDiscount(DiscountCreateRequestDTO request)
        {
            try
            {
                // Check if discount code already exists
                var existingDiscount = await _context.Discounts
                    .FirstOrDefaultAsync(d => d.DiscountCode == request.DiscountCode);

                if (existingDiscount != null)
                {
                    throw new InvalidOperationException("Mã discount đã tồn tại");
                }

                // Validate customer exists
                var customerExists = await _context.Customers
                    .AnyAsync(c => c.CustomerId == request.CustomerId);

                if (!customerExists)
                {
                    throw new InvalidOperationException("Khách hàng không tồn tại");
                }

                var discount = new BookingFlightServer.Entities.Discount
                {
                    DiscountCode = request.DiscountCode,
                    DiscountPercent = request.DiscountPercent,
                    DiscountTitle = request.DiscountTitle,
                    DiscountInfor = request.DiscountInfor,
                    CustomerId = request.CustomerId,
                    Status = request.Status
                };

                _context.Discounts.Add(discount);
                await _context.SaveChangesAsync();

                return new DiscountDTO
                {
                    DiscountId = discount.DiscountId,
                    DiscountCode = discount.DiscountCode,
                    DiscountPercent = discount.DiscountPercent,
                    DiscountTitle = discount.DiscountTitle,
                    DiscountInfor = discount.DiscountInfor,
                    CustomerId = discount.CustomerId,
                    Status = discount.Status
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo discount: {ex.Message}");
            }
        }

        public async Task<DiscountDTO> UpdateDiscount(DiscountUpdateRequestDTO request)
        {
            try
            {
                var discount = await _context.Discounts.FindAsync(request.DiscountId);
                if (discount == null)
                {
                    throw new InvalidOperationException("Discount không tồn tại");
                }

                // Check if discount code already exists (excluding current discount)
                var existingDiscount = await _context.Discounts
                    .FirstOrDefaultAsync(d => d.DiscountCode == request.DiscountCode && d.DiscountId != request.DiscountId);

                if (existingDiscount != null)
                {
                    throw new InvalidOperationException("Mã discount đã tồn tại");
                }

                // Validate customer exists
                var customerExists = await _context.Customers
                    .AnyAsync(c => c.CustomerId == request.CustomerId);

                if (!customerExists)
                {
                    throw new InvalidOperationException("Khách hàng không tồn tại");
                }

                discount.DiscountCode = request.DiscountCode;
                discount.DiscountPercent = request.DiscountPercent;
                discount.DiscountTitle = request.DiscountTitle;
                discount.DiscountInfor = request.DiscountInfor;
                discount.CustomerId = request.CustomerId;
                discount.Status = request.Status;

                await _context.SaveChangesAsync();

                return new DiscountDTO
                {
                    DiscountId = discount.DiscountId,
                    DiscountCode = discount.DiscountCode,
                    DiscountPercent = discount.DiscountPercent,
                    DiscountTitle = discount.DiscountTitle,
                    DiscountInfor = discount.DiscountInfor,
                    CustomerId = discount.CustomerId,
                    Status = discount.Status
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật discount: {ex.Message}");
            }
        }

        public async Task<bool> DeleteDiscount(int id)
        {
            try
            {
                var discount = await _context.Discounts.FindAsync(id);
                if (discount == null)
                {
                    return false;
                }

                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<object>> GetDiscountStatuses()
        {
            return new List<object>
            {
                new { statusId = 0, statusName = "Inactive" },
                new { statusId = 1, statusName = "Active" }
            };
        }
    }
}
