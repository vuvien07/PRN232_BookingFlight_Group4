using BookingFlightClient.Models.ViewModels;
using Library;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace BookingFlightClient.Controllers
{
    public class BookingHistoryController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BookingHistoryController> _logger;

        public BookingHistoryController(HttpClient httpClient, ILogger<BookingHistoryController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            try
            {
                // TEMP: For testing, try with a hardcoded customer ID first
                int? customerId = null;
                
                // Try to get customer ID from token
                customerId = await GetCurrentCustomerIdAsync();
                
                // TEMP: If no customer from token, try hardcoded ID for testing
                if (customerId == null)
                {
                    _logger.LogWarning("No customer ID from token, using test customer ID 1 for debugging");
                    customerId = 1; // Use customer ID 1 for testing
                }

                if (customerId == null)
                {
                    _logger.LogWarning("No customer ID available, redirecting to login");
                    return RedirectToAction("Index", "Login");
                }

                _logger.LogInformation("Using customer ID: {CustomerId}", customerId);

                // Get booking history from API
                var response = await _httpClient.GetAsync($"http://localhost:5077/api/Ticket/getByCustomerIdPaginated/{customerId}?page={page}&pageSize={pageSize}");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var bookingHistory = JsonSerializer.Deserialize<PaginatedTicketResult>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    ViewBag.BookingHistory = bookingHistory;
                    ViewBag.CurrentPage = page;
                    ViewBag.PageSize = pageSize;
                    
                    return View();
                }
                else
                {
                    _logger.LogError("Failed to get booking history. Status: {StatusCode}", response.StatusCode);
                    ViewBag.BookingHistory = new PaginatedTicketResult();
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading booking history");
                ViewBag.BookingHistory = new PaginatedTicketResult();
                return View();
            }
        }

        private async Task<int?> GetCurrentCustomerIdAsync()
        {
            try
            {
                // Get JWT token from cookie
                var token = Request.Cookies["jwtToken"];
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("JWT token not found in cookies");
                    return null;
                }

                _logger.LogInformation("JWT token found: {Token}", token.Substring(0, Math.Min(50, token.Length)) + "...");

                // Decode JWT token to get account ID
                var accountId = JwtDecoder.GetAccountIdFromToken(token);
                if (accountId == null)
                {
                    _logger.LogWarning("AccountId not found in JWT token");
                    return null;
                }

                _logger.LogInformation("AccountId found: {AccountId}", accountId);

                // Get customer by account ID from the new endpoint
                var response = await _httpClient.GetAsync($"http://localhost:5077/api/FlightCustomers/by-account/{accountId}");
                _logger.LogInformation("API Response Status: {StatusCode}", response.StatusCode);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Customer API Response: {Response}", jsonResponse);
                    
                    var customer = JsonSerializer.Deserialize<CustomerInfo>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    _logger.LogInformation("Customer ID: {CustomerId}", customer?.CustomerId);
                    return customer?.CustomerId;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("API Error: {StatusCode}, Content: {Content}", response.StatusCode, errorContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current customer ID");
                return null;
            }
        }

        public async Task<IActionResult> Details(int? ticketId, int? TicketId)
        {
            try
            {
                // Handle both route parameter (ticketId) and query parameter (TicketId)
                int id = ticketId ?? TicketId ?? 0;
                
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid ticket ID provided");
                    return BadRequest("Invalid ticket ID");
                }

                _logger.LogInformation("Fetching ticket details for ID: {TicketId}", id);
                
                var response = await _httpClient.GetAsync($"http://localhost:5077/api/Ticket/getTicketById/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Ticket API Response: {Response}", jsonResponse);
                    
                    var ticket = JsonSerializer.Deserialize<TicketViewModel>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    if (ticket == null)
                    {
                        _logger.LogWarning("Failed to deserialize ticket response");
                        return NotFound("Ticket not found");
                    }

                    return View(ticket);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading ticket details for ID: {TicketId}", ticketId);
                return NotFound();
            }
        }
    }

    // Helper classes for deserialization
    public class CustomerInfo
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class PaginatedTicketResult
    {
        public List<TicketViewModel> Tickets { get; set; } = new List<TicketViewModel>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class TicketViewModel
    {
        public int TicketId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string? ContactFullName { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactAddress { get; set; }
        public ClassSeatViewModel ClassSeatDTO { get; set; } = new();
        public CustomerViewModel CustomerDTO { get; set; } = new();
        public FlightViewModel FlightDTO { get; set; } = new();
        public List<TicketItemViewModel> TicketItems { get; set; } = new();
    }

    public class ClassSeatViewModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Description { get; set; }
    }

    public class CustomerViewModel
    {
        public int CustomerId { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class FlightViewModel
    {
        public int FlightId { get; set; }
        public string FlightCode { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public AirportViewModel DepartureAirport { get; set; } = new();
        public AirportViewModel ArrivalAirport { get; set; } = new();
        public PlaneViewModel Plane { get; set; } = new();
    }

    public class AirportViewModel
    {
        public int AirportId { get; set; }
        public string AirportCode { get; set; } = string.Empty;
        public string AirportName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class PlaneViewModel
    {
        public int PlaneId { get; set; }
        public string PlaneCode { get; set; } = string.Empty;
        public string PlaneModel { get; set; } = string.Empty;
    }

    public class TicketItemViewModel
    {
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class TicketDetailViewModel : TicketViewModel
    {
        // Additional properties for detailed view if needed
    }
}
