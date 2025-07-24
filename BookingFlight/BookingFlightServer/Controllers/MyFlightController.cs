using BookingFlightServer.DTO.Customer;
using BookingFlightServer.Services;
using Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BookingFlightServer.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MyFlightController : ControllerBase
    {
        private readonly IMyFlightService _myFlightService;
        private readonly BookingFlightContext _context;

        public MyFlightController(IMyFlightService myFlightService, BookingFlightContext context)
        {
            _myFlightService = myFlightService;
            _context = context;
        }

        /// <summary>
        /// Debug endpoint to test authentication without service calls
        /// </summary>
        /// <returns>Authentication debug info</returns>
        [HttpGet("auth-test")]
        public async Task<IActionResult> TestAuthentication()
        {
            try
            {
                Console.WriteLine("=== MyFlight AUTH TEST ===");
                Console.WriteLine($"Headers: {string.Join(", ", Request.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}"))}");
                Console.WriteLine($"Cookies: {string.Join(", ", Request.Cookies.Select(c => $"{c.Key}={c.Value}"))}");
                Console.WriteLine($"User Identity Name: {User.Identity?.Name ?? "NULL"}");
                Console.WriteLine($"User IsAuthenticated: {User.Identity?.IsAuthenticated ?? false}");
                Console.WriteLine($"Claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"))}");
                
                var customerId = await GetCurrentCustomerId();
                Console.WriteLine($"GetCurrentCustomerId result: {customerId}");
                
                return Ok(new
                {
                    success = true,
                    authenticated = User.Identity?.IsAuthenticated ?? false,
                    userName = User.Identity?.Name,
                    customerId = customerId,
                    claims = User.Claims.Select(c => new { Type = c.Type, Value = c.Value }).ToList(),
                    headers = Request.Headers.ToDictionary(h => h.Key, h => string.Join(",", h.Value)),
                    cookies = Request.Cookies.ToDictionary(c => c.Key, c => c.Value)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Auth test exception: {ex.Message}");
                return StatusCode(500, new { success = false, error = ex.Message, details = ex.ToString() });
            }
        }

        /// <summary>
        /// Get customer flights with filters
        /// </summary>
        /// <param name="request">Filter parameters</param>
        /// <returns>List of customer flights</returns>
        [HttpPost("flights")]
        public async Task<IActionResult> GetCustomerFlights([FromBody] MyFlightRequestDTO request)
        {
            try
            {
                var customerId = await GetCurrentCustomerId();
                if (customerId == null)
                {
                    return Unauthorized("Customer not found in token");
                }

                request.CustomerId = customerId.Value;
                var flights = await _myFlightService.GetCustomerFlightsAsync(request);

                return Ok(new
                {
                    success = true,
                    data = flights,
                    message = "Flights retrieved successfully",
                    count = flights.Count
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving flights", error = ex.Message });
            }
        }

        /// <summary>
        /// Get customer flights in calendar format
        /// </summary>
        /// <param name="year">Year (optional)</param>
        /// <param name="month">Month (optional)</param>
        /// <returns>Calendar view of flights</returns>
        [HttpGet("calendar")]
        public async Task<IActionResult> GetFlightsCalendar([FromQuery] int? year, [FromQuery] int? month)
        {
            try
            {
                // Debug logging
                Console.WriteLine("[MyFlight Calendar] === DEBUG AUTHENTICATION ===");
                Console.WriteLine($"[MyFlight Calendar] Year: {year}, Month: {month}");
                Console.WriteLine($"[MyFlight Calendar] Headers: {string.Join(", ", Request.Headers.Select(h => $"{h.Key}={h.Value}"))}");
                Console.WriteLine($"[MyFlight Calendar] Cookies: {string.Join(", ", Request.Cookies.Select(c => $"{c.Key}={c.Value}"))}");
                Console.WriteLine($"[MyFlight Calendar] User Identity: {User.Identity?.Name ?? "NULL"}");
                Console.WriteLine($"[MyFlight Calendar] User IsAuthenticated: {User.Identity?.IsAuthenticated ?? false}");
                Console.WriteLine($"[MyFlight Calendar] Claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"))}");

                var customerId = await GetCurrentCustomerId();
                Console.WriteLine($"[MyFlight Calendar] GetCurrentCustomerId result: {customerId}");
                
                if (customerId == null)
                {
                    Console.WriteLine("[MyFlight Calendar] Customer not found in token - returning 401");
                    return Unauthorized("Customer not found in token");
                }

                var calendar = await _myFlightService.GetCustomerFlightsCalendarAsync(customerId.Value, year, month);

                return Ok(new
                {
                    success = true,
                    data = calendar,
                    message = "Flight calendar retrieved successfully"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving flight calendar", error = ex.Message });
            }
        }

        /// <summary>
        /// Get flight detail by ticket ID
        /// </summary>
        /// <param name="ticketId">Ticket ID</param>
        /// <returns>Flight details</returns>
        [HttpGet("detail/{ticketId}")]
        public async Task<IActionResult> GetFlightDetail(int ticketId)
        {
            try
            {
                var customerId = await GetCurrentCustomerId();
                if (customerId == null)
                {
                    return Unauthorized("Customer not found in token");
                }
                
                // Get all flights for customer and find the specific one
                var request = new MyFlightRequestDTO { CustomerId = customerId.Value };
                var flights = await _myFlightService.GetCustomerFlightsAsync(request);
                var flight = flights.FirstOrDefault(f => f.TicketId == ticketId);
                
                if (flight == null)
                {
                    return NotFound(new { success = false, message = "Flight not found" });
                }
                
                return Ok(new { success = true, data = flight, message = "Flight detail retrieved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving flight detail", error = ex.Message });
            }
        }

        /// <summary>
        /// Get upcoming flights for customer
        /// </summary>
        /// <returns>List of upcoming flights</returns>
        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingFlights()
        {
            try
            {
                var customerId = await GetCurrentCustomerId();
                if (customerId == null)
                {
                    return Unauthorized("Customer not found in token");
                }

                var flights = await _myFlightService.GetUpcomingFlightsAsync(customerId.Value);

                return Ok(new
                {
                    success = true,
                    data = flights,
                    message = "Upcoming flights retrieved successfully",
                    count = flights.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving upcoming flights", error = ex.Message });
            }
        }

        /// <summary>
        /// Get past flights for customer
        /// </summary>
        /// <param name="limit">Number of records to return</param>
        /// <returns>List of past flights</returns>
        [HttpGet("past")]
        public async Task<IActionResult> GetPastFlights([FromQuery] int limit = 10)
        {
            try
            {
                var customerId = await GetCurrentCustomerId();
                if (customerId == null)
                {
                    return Unauthorized("Customer not found in token");
                }

                var flights = await _myFlightService.GetPastFlightsAsync(customerId.Value, limit);

                return Ok(new
                {
                    success = true,
                    data = flights,
                    message = "Past flights retrieved successfully",
                    count = flights.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving past flights", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all flights for customer (both upcoming and past)
        /// </summary>
        /// <returns>List of all flights</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllFlights()
        {
            try
            {
                var customerId = await GetCurrentCustomerId();
                if (customerId == null)
                {
                    return Unauthorized("Customer not found in token");
                }
                var request = new MyFlightRequestDTO { CustomerId = customerId.Value };
                var flights = await _myFlightService.GetCustomerFlightsAsync(request);
                return Ok(new { success = true, data = flights, message = "All flights retrieved successfully", count = flights.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving all flights", error = ex.Message });
            }
        }

        /// <summary>
        /// Get flight statistics for customer
        /// </summary>
        /// <returns>Flight statistics</returns>
        [HttpGet("stats")]
        public async Task<IActionResult> GetFlightStats()
        {
            try
            {
                // Debug logging
                Console.WriteLine("[MyFlight Stats] === DEBUG AUTHENTICATION ===");
                Console.WriteLine($"[MyFlight Stats] Headers: {string.Join(", ", Request.Headers.Select(h => $"{h.Key}={h.Value}"))}");
                Console.WriteLine($"[MyFlight Stats] Cookies: {string.Join(", ", Request.Cookies.Select(c => $"{c.Key}={c.Value}"))}");
                Console.WriteLine($"[MyFlight Stats] User Identity: {User.Identity?.Name ?? "NULL"}");
                Console.WriteLine($"[MyFlight Stats] User IsAuthenticated: {User.Identity?.IsAuthenticated ?? false}");
                Console.WriteLine($"[MyFlight Stats] Claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"))}");

                var customerId = await GetCurrentCustomerId();
                Console.WriteLine($"[MyFlight Stats] GetCurrentCustomerId result: {customerId}");
                
                if (customerId == null)
                {
                    Console.WriteLine("[MyFlight Stats] Customer not found in token - returning 401");
                    return Unauthorized("Customer not found in token");
                }

                var stats = await _myFlightService.GetFlightStatsAsync(customerId.Value);

                return Ok(new
                {
                    success = true,
                    data = stats,
                    message = "Flight statistics retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving flight statistics", error = ex.Message });
            }
        }

        /// <summary>
        /// Get flights grouped by status
        /// </summary>
        /// <returns>Flights grouped by status</returns>
        [HttpGet("grouped")]
        public async Task<IActionResult> GetFlightsGroupedByStatus()
        {
            try
            {
                var customerId = await GetCurrentCustomerId();
                if (customerId == null)
                {
                    return Unauthorized("Customer not found in token");
                }

                var groupedFlights = await _myFlightService.GetFlightsGroupedByStatusAsync(customerId.Value);

                return Ok(new
                {
                    success = true,
                    data = groupedFlights,
                    message = "Grouped flights retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving grouped flights", error = ex.Message });
            }
        }

        /// <summary>
        /// Search customer flights
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>Matching flights</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchFlights([FromQuery] string searchTerm)
        {
            try
            {
                var customerId = await GetCurrentCustomerId();
                if (customerId == null)
                {
                    return Unauthorized("Customer not found in token");
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { success = false, message = "Search term is required" });
                }

                var flights = await _myFlightService.SearchCustomerFlightsAsync(customerId.Value, searchTerm);

                return Ok(new
                {
                    success = true,
                    data = flights,
                    message = "Search completed successfully",
                    count = flights.Count,
                    searchTerm = searchTerm
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while searching flights", error = ex.Message });
            }
        }

        /// <summary>
        /// Validate customer access to a ticket
        /// </summary>
        /// <param name="ticketId">Ticket ID</param>
        /// <returns>Access validation result</returns>
        [HttpGet("validate-access/{ticketId}")]
        public async Task<IActionResult> ValidateAccess(int ticketId)
        {
            try
            {
                var customerId = await GetCurrentCustomerId();
                if (customerId == null)
                {
                    return Unauthorized("Customer not found in token");
                }

                var hasAccess = await _myFlightService.ValidateCustomerAccessAsync(ticketId, customerId.Value);

                return Ok(new
                {
                    success = true,
                    data = new { hasAccess = hasAccess, ticketId = ticketId },
                    message = hasAccess ? "Access granted" : "Access denied"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while validating access", error = ex.Message });
            }
        }

        /// <summary>
        /// Get current customer ID from JWT token
        /// </summary>
        /// <returns>Customer ID or null</returns>
        private async Task<int?> GetCurrentCustomerId()
        {
            try
            {
                Console.WriteLine("[GetCurrentCustomerId] === EXTRACTING CUSTOMER ID FROM TOKEN ===");
                
                // Get username from JWT token (this is what's actually in the token)
                var username = User.FindFirst(ClaimTypes.Name)?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
                Console.WriteLine($"[GetCurrentCustomerId] Username from token: {username}");
                
                if (string.IsNullOrEmpty(username))
                {
                    Console.WriteLine("[GetCurrentCustomerId] No username found in token claims");
                    return null;
                }

                // Find the customer by username through Account -> Customer relationship
                var customer = await _context.Customers
                    .Include(c => c.Account)
                    .FirstOrDefaultAsync(c => c.Account.Username == username);
                
                if (customer != null)
                {
                    Console.WriteLine($"[GetCurrentCustomerId] ✅ Found customer: ID={customer.CustomerId}, Name={customer.Fullname}");
                    return customer.CustomerId;
                }
                
                Console.WriteLine($"[GetCurrentCustomerId] ❌ No customer found with username: {username}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetCurrentCustomerId] Exception: {ex.Message}");
                Console.WriteLine($"[GetCurrentCustomerId] Stack trace: {ex.StackTrace}");
                return null;
            }
        }
    }
}
