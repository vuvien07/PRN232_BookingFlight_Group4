using BookingFlightServer.DTO.Manager;
using BookingFlightServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager")]
    public class ManagerController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly IItemService _itemService;
        
        public ManagerController(IServiceService serviceService, IItemService itemService)
        {
            _serviceService = serviceService;
            _itemService = itemService;
        }

        [HttpPost("services/list")]
        [AllowAnonymous] // Temporary for testing
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

        [HttpPost("services")]
        [AllowAnonymous] // Temporary for testing
        public async Task<IActionResult> CreateService([FromBody] ServiceCreateRequestDTO request)
        {
            try
            {
                Console.WriteLine($"Received service creation request: {System.Text.Json.JsonSerializer.Serialize(request)}");
                var service = await _serviceService.CreateService(request);
                Console.WriteLine($"Service created successfully: {service.ServiceId}");
                return Ok(new { success = true, data = service, message = "Service created successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating service: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
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

        [HttpDelete("services/{id}")]
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
        [AllowAnonymous] // Temporary for testing
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
        [AllowAnonymous] // Temporary for testing
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
        [AllowAnonymous] // Temporary for testing
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
        [AllowAnonymous] // Temporary for testing
        public async Task<IActionResult> CreateItem([FromBody] ItemCreateRequestDTO request)
        {
            try
            {
                Console.WriteLine($"Received item creation request: {System.Text.Json.JsonSerializer.Serialize(request)}");
                
                if (string.IsNullOrEmpty(request?.ItemName))
                {
                    return BadRequest(new { success = false, message = "Item name is required" });
                }
                
                var item = await _itemService.CreateItem(request);
                Console.WriteLine($"Item created successfully: {item.ItemId}");
                return Ok(new { success = true, data = item, message = "Item created successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating item: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
