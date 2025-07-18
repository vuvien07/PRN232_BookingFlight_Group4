using BookingFlightServer.Data;
using BookingFlightServer.DTO.Manager;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Services
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
                query = query.Where(d => d.DiscountCode.Contains(request.SearchTerm) ||
                                   d.DiscountTitle.Contains(request.SearchTerm) ||
                                   (d.Customer.Account.Username != null && d.Customer.Account.Username.Contains(request.SearchTerm)));
            }

            if (request.Status.HasValue)
            {
                query = query.Where(d => d.Status == request.Status);
            }

            if (request.CustomerId.HasValue)
            {
                query = query.Where(d => d.CustomerId == request.CustomerId);
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
                    Status = d.Status,
                    CustomerId = d.CustomerId,
                    CustomerName = d.Customer.Account.Username
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

            // Apply filters
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(d => d.DiscountCode.Contains(request.SearchTerm) ||
                                   d.DiscountTitle.Contains(request.SearchTerm) ||
                                   (d.Customer.Account.Username != null && d.Customer.Account.Username.Contains(request.SearchTerm)));
            }

            if (request.Status.HasValue)
            {
                query = query.Where(d => d.Status == request.Status);
            }

            if (request.CustomerId.HasValue)
            {
                query = query.Where(d => d.CustomerId == request.CustomerId);
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
                    Status = d.Status,
                    CustomerId = d.CustomerId,
                    CustomerName = d.Customer.Account.Username
                })
                .FirstOrDefaultAsync();

            return discount;
        }

        public async Task<DiscountDTO> CreateDiscount(DiscountCreateRequestDTO request)
        {
            // Check if discount code already exists
            if (await IsDiscountCodeExists(request.DiscountCode))
            {
                throw new InvalidOperationException("Discount code already exists");
            }

            // Validate customer exists
            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == request.CustomerId);
            if (!customerExists)
            {
                throw new InvalidOperationException("Customer not found");
            }

            var discount = new Discount
            {
                DiscountCode = request.DiscountCode,
                DiscountPercent = request.DiscountPercent,
                DiscountTitle = request.DiscountTitle,
                DiscountInfor = request.DiscountInfor,
                Status = request.Status,
                CustomerId = request.CustomerId
            };

            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();

            return await GetDiscountById(discount.DiscountId) ?? throw new InvalidOperationException("Failed to create discount");
        }

        public async Task<DiscountDTO> UpdateDiscount(DiscountUpdateRequestDTO request)
        {
            var discount = await _context.Discounts.FindAsync(request.DiscountId);
            if (discount == null)
            {
                throw new InvalidOperationException("Discount not found");
            }

            // Check if discount code already exists (excluding current discount)
            if (await IsDiscountCodeExists(request.DiscountCode, request.DiscountId))
            {
                throw new InvalidOperationException("Discount code already exists");
            }

            // Validate customer exists
            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == request.CustomerId);
            if (!customerExists)
            {
                throw new InvalidOperationException("Customer not found");
            }

            discount.DiscountCode = request.DiscountCode;
            discount.DiscountPercent = request.DiscountPercent;
            discount.DiscountTitle = request.DiscountTitle;
            discount.DiscountInfor = request.DiscountInfor;
            discount.Status = request.Status;
            discount.CustomerId = request.CustomerId;

            await _context.SaveChangesAsync();

            return await GetDiscountById(discount.DiscountId) ?? throw new InvalidOperationException("Failed to update discount");
        }

        public async Task<bool> DeleteDiscount(int id)
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

        public async Task<List<object>> GetDiscountStatuses()
        {
            return await Task.FromResult(new List<object>
            {
                new { StatusId = 1, StatusName = "Active" },
                new { StatusId = 0, StatusName = "Inactive" }
            });
        }

        public async Task<bool> IsDiscountCodeExists(string code, int? excludeId = null)
        {
            try
            {
                var query = _context.Discounts.Where(d => d.DiscountCode == code);
                
                if (excludeId.HasValue)
                {
                    query = query.Where(d => d.DiscountId != excludeId.Value);
                }

                return await query.AnyAsync();
            }
            catch
            {
                return false;
            }
        }
    }
}
