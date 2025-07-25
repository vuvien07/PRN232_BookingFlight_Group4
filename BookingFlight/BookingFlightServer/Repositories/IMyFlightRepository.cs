using BookingFlightServer.DTO.Customer;
using BookingFlightServer.Entities;

namespace BookingFlightServer.Repositories
{
    public interface IMyFlightRepository
    {
        /// <summary>
        /// Get customer flights with full join information
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="startDate">Start date filter</param>
        /// <param name="endDate">End date filter</param>
        /// <param name="statusId">Status filter</param>
        /// <returns>List of flights with full information</returns>
        Task<List<MyFlightResponseDTO>> GetCustomerFlightsAsync(int customerId, DateTime? startDate = null, DateTime? endDate = null, int? statusId = null);

        /// <summary>
        /// Get customer flights grouped by date for calendar view
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="year">Year</param>
        /// <param name="month">Month</param>
        /// <returns>Calendar data with flights grouped by date</returns>
        Task<MyFlightCalendarResponseDTO> GetCustomerFlightsCalendarAsync(int customerId, int year, int month);

        /// <summary>
        /// Get customer flight details by ticket ID
        /// </summary>
        /// <param name="ticketId">Ticket ID</param>
        /// <param name="customerId">Customer ID for security check</param>
        /// <returns>Flight details</returns>
        Task<MyFlightResponseDTO?> GetFlightDetailAsync(int ticketId, int customerId);

        /// <summary>
        /// Check if customer owns the ticket
        /// </summary>
        /// <param name="ticketId">Ticket ID</param>
        /// <param name="customerId">Customer ID</param>
        /// <returns>True if customer owns the ticket</returns>
        Task<bool> IsTicketBelongToCustomerAsync(int ticketId, int customerId);

        /// <summary>
        /// Get upcoming flights for customer (next 30 days)
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
        /// Get flight statistics for customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>Flight statistics</returns>
        Task<CustomerFlightStatsDTO> GetFlightStatsAsync(int customerId);
    }
}
