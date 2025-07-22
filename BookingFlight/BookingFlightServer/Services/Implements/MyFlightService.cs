using AutoMapper;
using BookingFlightServer.DTO.Customer;
using BookingFlightServer.Repositories;

namespace BookingFlightServer.Services.Implements
{
    public class MyFlightService : IMyFlightService
    {
        private readonly IMyFlightRepository _myFlightRepository;
        private readonly IMapper _mapper;

        public MyFlightService(IMyFlightRepository myFlightRepository, IMapper mapper)
        {
            _myFlightRepository = myFlightRepository;
            _mapper = mapper;
        }

        public async Task<List<MyFlightResponseDTO>> GetCustomerFlightsAsync(MyFlightRequestDTO request)
        {
            if (request.CustomerId == null || request.CustomerId <= 0)
            {
                throw new ArgumentException("Customer ID is required and must be valid.");
            }

            return await _myFlightRepository.GetCustomerFlightsAsync(
                request.CustomerId.Value,
                request.StartDate,
                request.EndDate,
                request.StatusId
            );
        }

        public async Task<MyFlightCalendarResponseDTO> GetCustomerFlightsCalendarAsync(int customerId, int? year = null, int? month = null)
        {
            if (customerId <= 0)
            {
                throw new ArgumentException("Customer ID must be valid.");
            }

            var currentDate = DateTime.Now;
            var targetYear = year ?? currentDate.Year;
            var targetMonth = month ?? currentDate.Month;

            return await _myFlightRepository.GetCustomerFlightsCalendarAsync(customerId, targetYear, targetMonth);
        }

        public async Task<MyFlightResponseDTO?> GetFlightDetailAsync(int ticketId, int customerId)
        {
            if (ticketId <= 0 || customerId <= 0)
            {
                throw new ArgumentException("Ticket ID and Customer ID must be valid.");
            }

            return await _myFlightRepository.GetFlightDetailAsync(ticketId, customerId);
        }

        public async Task<List<MyFlightResponseDTO>> GetUpcomingFlightsAsync(int customerId)
        {
            if (customerId <= 0)
            {
                throw new ArgumentException("Customer ID must be valid.");
            }

            return await _myFlightRepository.GetUpcomingFlightsAsync(customerId);
        }

        public async Task<List<MyFlightResponseDTO>> GetPastFlightsAsync(int customerId, int limit = 10)
        {
            if (customerId <= 0)
            {
                throw new ArgumentException("Customer ID must be valid.");
            }

            if (limit <= 0)
            {
                limit = 10;
            }

            return await _myFlightRepository.GetPastFlightsAsync(customerId, limit);
        }

        public async Task<CustomerFlightStatsDTO> GetFlightStatsAsync(int customerId)
        {
            if (customerId <= 0)
            {
                throw new ArgumentException("Customer ID must be valid.");
            }

            return await _myFlightRepository.GetFlightStatsAsync(customerId);
        }

        public async Task<bool> ValidateCustomerAccessAsync(int ticketId, int customerId)
        {
            if (ticketId <= 0 || customerId <= 0)
            {
                return false;
            }

            return await _myFlightRepository.IsTicketBelongToCustomerAsync(ticketId, customerId);
        }

        public async Task<CustomerFlightsGroupedDTO> GetFlightsGroupedByStatusAsync(int customerId)
        {
            if (customerId <= 0)
            {
                throw new ArgumentException("Customer ID must be valid.");
            }

            var allFlights = await _myFlightRepository.GetCustomerFlightsAsync(customerId);
            var stats = await _myFlightRepository.GetFlightStatsAsync(customerId);

            var now = DateTime.Now;

            return new CustomerFlightsGroupedDTO
            {
                UpcomingFlights = allFlights
                    .Where(f => f.DepartureTime >= now && f.StatusId == 1)
                    .OrderBy(f => f.DepartureTime)
                    .ToList(),
                
                CompletedFlights = allFlights
                    .Where(f => f.DepartureTime < now && f.StatusId == 1)
                    .OrderByDescending(f => f.DepartureTime)
                    .ToList(),
                
                CancelledFlights = allFlights
                    .Where(f => f.StatusId == 2) // Assuming 2 = Cancelled
                    .OrderByDescending(f => f.BookingDate)
                    .ToList(),
                
                Stats = stats
            };
        }

        public async Task<List<MyFlightResponseDTO>> SearchCustomerFlightsAsync(int customerId, string searchTerm)
        {
            if (customerId <= 0)
            {
                throw new ArgumentException("Customer ID must be valid.");
            }

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<MyFlightResponseDTO>();
            }

            var allFlights = await _myFlightRepository.GetCustomerFlightsAsync(customerId);
            var searchTermLower = searchTerm.ToLower().Trim();

            return allFlights.Where(f =>
                f.FlightCode.ToLower().Contains(searchTermLower) ||
                f.DepartureAirportName.ToLower().Contains(searchTermLower) ||
                f.ArrivalAirportName.ToLower().Contains(searchTermLower) ||
                f.DepartureAirportCode.ToLower().Contains(searchTermLower) ||
                f.ArrivalAirportCode.ToLower().Contains(searchTermLower) ||
                f.TicketNumber.ToLower().Contains(searchTermLower)
            ).ToList();
        }
    }
}
