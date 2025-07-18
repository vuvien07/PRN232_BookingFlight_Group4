using BookingFlightServer.DTO.Manager;
using BookingFlightServer.Services;
using BookingFlightServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = "Manager")] // Temporary disable for testing
    public class ManagerController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly IItemService _itemService;
        private readonly IPlaneService _planeService;
        private readonly IDiscountService _discountService;
        private readonly BookingFlightContext _context;
        
        public ManagerController(IServiceService serviceService, IItemService itemService, IPlaneService planeService, IDiscountService discountService, BookingFlightContext context)
        {
            _serviceService = serviceService;
            _itemService = itemService;
            _planeService = planeService;
            _discountService = discountService;
            _context = context;
        }

        [HttpPost("services/list")]

        public async Task<IActionResult> GetServices([FromBody] ServiceListRequestDTO request)
        {
            try
            {
                var services = await _serviceService.GetServicesByFilter(request);
                var totalCount = await _serviceService.GetTotalServicesCount(request);
                var totalPages = Math.Ceiling((double)totalCount / request.PageSize);

                return Ok(new
                {
                    success = true,
                    data = services,
                    pagination = new
                    {
                        currentPage = request.Page,
                        pageSize = request.PageSize,
                        totalCount = totalCount,
                        totalPages = totalPages
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("services/my-services")]
        public async Task<IActionResult> GetMyServices()
        {
            try
            {
                var managerIdClaim = HttpContext.User?.FindFirst("ManagerId")?.Value;
                if (string.IsNullOrEmpty(managerIdClaim) || !int.TryParse(managerIdClaim, out int managerId))
                {
                    return Unauthorized(new { success = false, message = "Manager ID not found in token" });
                }

                var services = await _serviceService.GetServicesByManagerId(managerId);
                return Ok(new { success = true, data = services });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("services/{id}")]
        public async Task<IActionResult> GetService(int id)
        {
            try
            {
                var service = await _serviceService.GetServiceById(id);
                if (service == null)
                {
                    return NotFound(new { success = false, message = "Service not found" });
                }

                return Ok(new { success = true, data = service });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("services/{id}/details")]
        [AllowAnonymous] // Temporary for testing
        public async Task<IActionResult> GetServiceDetails(int id)
        {
            try
            {
                var serviceDetails = await _serviceService.GetServiceDetails(id);
                if (serviceDetails == null)
                {
                    return NotFound(new { success = false, message = "Service not found" });
                }

                return Ok(new { success = true, data = serviceDetails });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("services")]
        public async Task<IActionResult> CreateService([FromBody] ServiceCreateRequestDTO request)
        {
            try
            {
                var service = await _serviceService.CreateService(request);
                return Ok(new { success = true, data = service, message = "Service created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("services/{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceUpdateRequestDTO request)
        {
            try
            {
                if (id != request.ServiceId)
                {
                    return BadRequest(new { success = false, message = "Service ID mismatch" });
                }

                var service = await _serviceService.UpdateService(request);
                return Ok(new { success = true, data = service, message = "Service updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("services/{id}/advanced")]
        [AllowAnonymous] // Temporary for testing
        public async Task<IActionResult> UpdateServiceAdvanced(int id, [FromBody] ServiceUpdateAdvancedRequestDTO request)
        {
            try
            {
                if (id != request.ServiceId)
                {
                    return BadRequest(new { success = false, message = "Service ID mismatch" });
                }

                var serviceDetails = await _serviceService.UpdateServiceAdvanced(request);
                return Ok(new { success = true, data = serviceDetails, message = "Service updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("services/{id}")]
        [AllowAnonymous] // Temporary for testing
        public async Task<IActionResult> DeleteService(int id)
        {
            try
            {
                var result = await _serviceService.DeleteService(id);
                if (!result)
                {
                    return NotFound(new { success = false, message = "Service not found" });
                }

                return Ok(new { success = true, message = "Service deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("services/statuses")]

        public async Task<IActionResult> GetServiceStatuses()
        {
            try
            {
                var statuses = await _serviceService.GetServiceStatuses();
                return Ok(new { success = true, data = statuses });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("items")]

        public async Task<IActionResult> GetActiveItems()
        {
            try
            {
                var items = await _itemService.GetActiveItems();
                return Ok(new { success = true, data = items });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("items/list")]

        public async Task<IActionResult> GetItems([FromBody] ItemListRequestDTO request)
        {
            try
            {
                var items = await _itemService.GetItemsByFilter(request);
                var totalCount = await _itemService.GetTotalItemsCount(request);
                var totalPages = Math.Ceiling((double)totalCount / request.PageSize);

                return Ok(new
                {
                    success = true,
                    data = items,
                    pagination = new
                    {
                        currentPage = request.Page,
                        pageSize = request.PageSize,
                        totalCount = totalCount,
                        totalPages = totalPages
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("items")]
        public async Task<IActionResult> CreateItem([FromBody] ItemCreateRequestDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.ItemName))
                {
                    return BadRequest(new { success = false, message = "Item name is required" });
                }
                
                var item = await _itemService.CreateItem(request);
                return Ok(new { success = true, data = item, message = "Item created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // PLANE MANAGEMENT ENDPOINTS
        [HttpPost("planes/list")]
        public async Task<IActionResult> GetPlanes([FromBody] PlaneListRequestDTO request)
        {
            try
            {
                var planes = await _planeService.GetPlanesWithFiltersAsync(request);
                var totalCount = await _planeService.GetTotalPlanesCountAsync(request);
                var totalPages = Math.Ceiling((double)totalCount / request.PageSize);

                return Ok(new
                {
                    success = true,
                    data = planes,
                    pagination = new
                    {
                        currentPage = request.Page,
                        pageSize = request.PageSize,
                        totalCount = totalCount,
                        totalPages = totalPages
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("planes/my-planes")]
        public async Task<IActionResult> GetMyPlanes()
        {
            try
            {
                var managerIdClaim = HttpContext.User?.FindFirst("ManagerId")?.Value;
                if (string.IsNullOrEmpty(managerIdClaim) || !int.TryParse(managerIdClaim, out int managerId))
                {
                    return Unauthorized(new { success = false, message = "Manager ID not found in token" });
                }

                var planes = await _planeService.GetPlanesByManagerIdAsync(managerId);
                return Ok(new { success = true, data = planes });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("planes/{id}")]
        public async Task<IActionResult> GetPlane(int id)
        {
            try
            {
                var plane = await _planeService.GetPlaneByIdAsync(id);
                if (plane == null)
                {
                    return NotFound(new { success = false, message = "Plane not found" });
                }

                return Ok(new { success = true, data = plane });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        [HttpPost("planes")]
        public async Task<IActionResult> CreatePlane([FromBody] PlaneCreateRequestDTO request)
        {
            try
            {
                // Debug: Check what claims are in the token
                var claims = HttpContext.User?.Claims?.ToList();
                if (claims != null)
                {
                    Console.WriteLine("Available claims in token:");
                    foreach (var claim in claims)
                    {
                        Console.WriteLine($"- {claim.Type}: {claim.Value}");
                    }
                }

                // Get Manager ID from JWT token
                var managerIdClaim = HttpContext.User?.FindFirst("ManagerId")?.Value;
                if (string.IsNullOrEmpty(managerIdClaim) || !int.TryParse(managerIdClaim, out int managerId))
                {
                    // Temporary fallback: use a default manager ID for testing
                    Console.WriteLine("ManagerId not found in token, using default manager ID = 1 for testing");
                    managerId = 1; // Temporary for testing
                    
                    // Comment out the return to allow testing
                    // return Unauthorized(new { success = false, message = "Manager ID not found in token" });
                }

                // Set the Manager ID from token
                request.ManagerId = managerId;

                Console.WriteLine($"Creating plane with ManagerId: {managerId}");
                var plane = await _planeService.CreatePlaneAsync(request);
                return Ok(new { success = true, data = plane, message = "Plane created successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating plane: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("planes/{id}")]
        public async Task<IActionResult> UpdatePlane(int id, [FromBody] PlaneUpdateRequestDTO request)
        {
            try
            {
                Console.WriteLine($"UpdatePlane endpoint called - ID: {id}");
                Console.WriteLine($"Request data: PlaneId={request.PlaneId}, PlaneCode={request.PlaneCode}, Model={request.Model}, Manufacture={request.Manufacture}, Year={request.YearOfManufacture}, StatusId={request.StatusId}, ManagerId={request.ManagerId}");
                
                if (id != request.PlaneId)
                {
                    Console.WriteLine($"ID mismatch: URL id={id}, Request PlaneId={request.PlaneId}");
                    return BadRequest(new { success = false, message = "Plane ID mismatch" });
                }

                var plane = await _planeService.UpdatePlaneAsync(request);
                Console.WriteLine($"Plane updated successfully: {plane.PlaneCode}");
                return Ok(new { success = true, data = plane, message = "Plane updated successfully" });
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating plane: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("items/{id}")]
        [AllowAnonymous] // Temporary for testing
        public async Task<IActionResult> GetItem(int id)
        {
            try
            {
                var item = await _itemService.GetItemById(id);
                if (item == null)
                {
                    return NotFound(new { success = false, message = "Item not found" });
                }

                return Ok(new { success = true, data = item });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("items/{id}")]
        [AllowAnonymous] // Temporary for testing
        public async Task<IActionResult> UpdateItem(int id, [FromBody] ItemUpdateRequestDTO request)
        {
            try
            {
                if (id != request.ItemId)
                {
                    return BadRequest(new { success = false, message = "Item ID mismatch" });
                }

                var item = await _itemService.UpdateItem(request);
                return Ok(new { success = true, data = item, message = "Item updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("planes/{id}")]
        public async Task<IActionResult> DeletePlane(int id)
        {
            try
            {
                var result = await _planeService.DeletePlaneAsync(id);
                if (!result)
                {
                    return NotFound(new { success = false, message = "Plane not found" });
                }

                return Ok(new { success = true, message = "Plane deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("items/{id}")]
        [AllowAnonymous] // Temporary for testing
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                var result = await _itemService.DeleteItem(id);
                if (!result)
                {
                    return NotFound(new { success = false, message = "Item not found" });
                }

                return Ok(new { success = true, message = "Item deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
                
        [HttpGet("planes/{id}/can-delete")]
        public async Task<IActionResult> CanDeletePlane(int id)
        {
            try
            {
                var canDelete = await _planeService.CanDeletePlaneAsync(id);
                return Ok(new { success = true, canDelete = canDelete.CanDelete, message = canDelete.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("items/statuses")]
        [AllowAnonymous] // Temporary for testing
        public async Task<IActionResult> GetItemStatuses()
        {
            try
            {
                // Return common statuses for items
                var statuses = new[]
                {
                    new { StatusId = 1, StatusType = "Active" },
                    new { StatusId = 2, StatusType = "Inactive" }
                };

                return Ok(new { success = true, data = statuses });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("planes/statuses")]
        public async Task<IActionResult> GetPlaneStatuses()
        {
            try
            {
                var statuses = await _planeService.GetPlaneStatusesAsync();

                return Ok(new { success = true, data = statuses });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // DISCOUNT MANAGEMENT ENDPOINTS
        
        [HttpPost("discounts/list")]
        [AllowAnonymous] // Temporary for testing
        public async Task<IActionResult> GetDiscounts([FromBody] DiscountListRequestDTO request)
        {
            try
            {
                Console.WriteLine($"GetDiscounts called with: Page={request?.Page}, PageSize={request?.PageSize}");
                
                // Get all discounts from database directly
                var discounts = await _context.Discounts
                    .Include(d => d.Customer)
                    .ThenInclude(c => c.Account)
                    .OrderByDescending(d => d.DiscountId)
                    .Take(10)
                    .Select(d => new 
                    {
                        discountId = d.DiscountId,
                        discountCode = d.DiscountCode,
                        discountTitle = d.DiscountTitle,
                        discountPercent = d.DiscountPercent,
                        customerName = d.Customer.Account.Username ?? "N/A",
                        status = d.Status
                    })
                    .ToListAsync();

                Console.WriteLine($"Found {discounts.Count} discounts");

                return Ok(new
                {
                    success = true,
                    data = discounts,
                    pagination = new
                    {
                        currentPage = 1,
                        pageSize = 10,
                        totalCount = discounts.Count,
                        totalPages = 1
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetDiscounts: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { success = false, message = $"Lỗi server: {ex.Message}" });
            }
        }

        [HttpGet("discounts/{id}")]
        [AllowAnonymous] // Temporary for testing
        public async Task<IActionResult> GetDiscount(int id)
        {
            try
            {
                var discount = await _discountService.GetDiscountById(id);
                if (discount == null)
                {
                    return NotFound(new { success = false, message = "Discount not found" });
                }

                return Ok(new { success = true, data = discount });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("discounts")]
        [AllowAnonymous] // Temporary for testing
        public async Task<IActionResult> CreateDiscount([FromBody] DiscountCreateRequestDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.DiscountCode))
                {
                    return BadRequest(new { success = false, message = "Discount code is required" });
                }

                if (request.DiscountPercent <= 0 || request.DiscountPercent > 100)
                {
                    return BadRequest(new { success = false, message = "Discount percent must be between 0 and 100" });
                }

                var discount = await _discountService.CreateDiscount(request);
                return Ok(new { success = true, data = discount, message = "Discount created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("discounts/{id}")]
        [AllowAnonymous] // Temporary for testing
        public async Task<IActionResult> UpdateDiscount(int id, [FromBody] DiscountUpdateRequestDTO request)
        {
            try
            {
                if (id != request.DiscountId)
                {
                    return BadRequest(new { success = false, message = "Discount ID mismatch" });
                }

                var discount = await _discountService.UpdateDiscount(request);
                return Ok(new { success = true, data = discount, message = "Discount updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("discounts/{id}")]
        [AllowAnonymous] // Temporary for testing
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            try
            {
                var result = await _discountService.DeleteDiscount(id);
                if (!result)
                {
                    return NotFound(new { success = false, message = "Discount not found" });
                }

                return Ok(new { success = true, message = "Discount deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("discounts/statuses")]
        [AllowAnonymous] // Temporary for testing
        public async Task<IActionResult> GetDiscountStatuses()
        {
            try
            {
                var statuses = await _discountService.GetDiscountStatuses();
                return Ok(new { success = true, data = statuses });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("customers")]
        [AllowAnonymous] // Temporary for testing
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var customers = await _context.Customers
                    .Include(c => c.Account)
                    .Where(c => c.Account != null)
                    .Select(c => new
                    {
                        CustomerId = c.CustomerId,
                        Username = c.Account.Username
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = customers });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("discounts/test")]
        [AllowAnonymous]
        public async Task<IActionResult> TestDiscounts()
        {
            try
            {
                Console.WriteLine("Testing database connection...");
                
                // Test basic query
                var discountCount = await _context.Discounts.CountAsync();
                Console.WriteLine($"Total discounts in DB: {discountCount}");
                
                // Test with customers
                var discountsWithCustomers = await _context.Discounts
                    .Include(d => d.Customer)
                        .ThenInclude(c => c.Account)
                    .Take(3)
                    .ToListAsync();
                
                Console.WriteLine($"Found {discountsWithCustomers.Count} discounts with customers");
                
                var result = discountsWithCustomers.Select(d => new {
                    id = d.DiscountId,
                    code = d.DiscountCode,
                    title = d.DiscountTitle,
                    customerName = d.Customer?.Account?.Username ?? "No customer"
                }).ToList();
                
                return Ok(new { 
                    success = true, 
                    totalCount = discountCount,
                    sampleData = result,
                    message = "Database connection OK" 
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database test failed: {ex.Message}");
                return StatusCode(500, new { 
                    success = false, 
                    message = ex.Message,
                    stackTrace = ex.StackTrace 
                });
            }
        }

        // Simple test endpoints without database
        [HttpGet("test/simple")]
        [AllowAnonymous]
        public IActionResult SimpleTest()
        {
            return Ok(new { success = true, message = "Simple test OK", time = DateTime.Now });
        }

        [HttpPost("discounts/dummy")]
        [AllowAnonymous]
        public IActionResult GetDummyDiscounts()
        {
            var dummyData = new List<object>
            {
                new {
                    discountId = 1,
                    discountCode = "SAVE10",
                    discountTitle = "Giảm giá 10%",
                    discountPercent = 10.0,
                    customerName = "Nguyễn Văn A",
                    status = 1
                },
                new {
                    discountId = 2,
                    discountCode = "SAVE20", 
                    discountTitle = "Giảm giá 20%",
                    discountPercent = 20.0,
                    customerName = "Trần Thị B",
                    status = 1
                }
            };

            return Ok(new
            {
                success = true,
                data = dummyData,
                pagination = new
                {
                    currentPage = 1,
                    pageSize = 10,
                    totalCount = 2,
                    totalPages = 1
                }
            });
        }
    }
}
