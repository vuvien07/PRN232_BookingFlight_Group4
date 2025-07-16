using BookingFlightServer.DTO.Manager;
using BookingFlightServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        
        public ManagerController(IServiceService serviceService, IItemService itemService, IPlaneService planeService)
        {
            _serviceService = serviceService;
            _itemService = itemService;
            _planeService = planeService;
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
    }
}
