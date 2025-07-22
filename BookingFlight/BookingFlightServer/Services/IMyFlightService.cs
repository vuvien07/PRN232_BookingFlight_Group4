using BookingFlightServer.DTO.Customer;

namespace BookingFlightServer.Services
{
    public interface IMyFlightService
    {
        /// <summary>
        /// Get customer flights based on various filters
        /// </summary>
        /// <param name="request">Filter parameters</param>
        /// <returns>List of customer flights</returns>
        Task<List<MyFlightResponseDTO>> GetCustomerFlightsAsync(MyFlightRequestDTO request);

        /// <summary>
        /// Get customer flights in calendar format
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="year">Year (default: current year)</param>
        /// <param name="month">Month (default: current month)</param>
        /// <returns>Calendar view of flights</returns>
        Task<MyFlightCalendarResponseDTO> GetCustomerFlightsCalendarAsync(int customerId, int? year = null, int? month = null);

        /// <summary>
        /// Get flight detail by ticket ID
        /// </summary>
        /// <param name="ticketId">Ticket ID</param>
        /// <param name="customerId">Customer ID for security check</param>
        /// <returns>Flight details or null</returns>
        Task<MyFlightResponseDTO?> GetFlightDetailAsync(int ticketId, int customerId);

        /// <summary>
        /// Get upcoming flights for customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>List of upcoming flights</returns>
        Task<List<MyFlightResponseDTO>> GetUpcomingFlightsAsync(int customerId);

        /// <summary>
        /// Get past flights for customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="limit">Number of records to return</param>
        /// <returns>List of past flights</returns>
        Task<List<MyFlightResponseDTO>> GetPastFlightsAsync(int customerId, int limit = 10);

        /// <summary>
        /// Get flight statistics for customer dashboard
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>Flight statistics</returns>
        Task<CustomerFlightStatsDTO> GetFlightStatsAsync(int customerId);

        /// <summary>
        /// Validate customer access to ticket
        /// </summary>
        /// <param name="ticketId">Ticket ID</param>
        /// <param name="customerId">Customer ID</param>
        /// <returns>True if customer can access the ticket</returns>
        Task<bool> ValidateCustomerAccessAsync(int ticketId, int customerId);

        /// <summary>
        /// Get flights grouped by status for customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>Flights grouped by status</returns>
        Task<CustomerFlightsGroupedDTO> GetFlightsGroupedByStatusAsync(int customerId);

        /// <summary>
        /// Search customer flights by flight code, destination, etc.
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>Matching flights</returns>
        Task<List<MyFlightResponseDTO>> SearchCustomerFlightsAsync(int customerId, string searchTerm);
    }
}
