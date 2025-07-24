using BookingFlightServer.Entities;
using BookingFlightServer.DTO.Request;
using BookingFlightServer.DTO.Response;
using BookingFlightServer.Data;
using BookingFlightServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingFlightServer.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly BookingFlightContext _context;
        private readonly IFeedbackTicketMappingService _mappingService;
        private readonly ILogger<FeedbackRepository> _logger;

        public FeedbackRepository(BookingFlightContext context, IFeedbackTicketMappingService mappingService, ILogger<FeedbackRepository> logger)
        {
            _context = context;
            _mappingService = mappingService;
            _logger = logger;
        }

        public async Task<List<Feedback>> GetAllAsync()
        {
            return await _context.Feedbacks
                .Include(f => f.Account)
                .ThenInclude(a => a.Customer)
                .Include(f => f.Account)
                .ThenInclude(a => a.Admin)
                .OrderByDescending(f => f.CreateAt)
                .ToListAsync();
        }

        public async Task<Feedback?> GetByIdAsync(int id)
        {
            return await _context.Feedbacks
                .Include(f => f.Account)
                .ThenInclude(a => a.Customer)
                .Include(f => f.Account)
                .ThenInclude(a => a.Admin)
                .FirstOrDefaultAsync(f => f.FeedbackId == id);
        }

        public async Task<Feedback> CreateAsync(Feedback feedback)
        {
            feedback.CreateAt = DateOnly.FromDateTime(DateTime.Now);
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
            
            // Load the account information
            await _context.Entry(feedback)
                .Reference(f => f.Account)
                .LoadAsync();
            
            if (feedback.Account != null)
            {
                await _context.Entry(feedback.Account)
                    .Reference(a => a.Customer)
                    .LoadAsync();
                await _context.Entry(feedback.Account)
                    .Reference(a => a.Admin)
                    .LoadAsync();
            }
            
            return feedback;
        }

        public async Task<Feedback> UpdateAsync(Feedback feedback)
        {
            _context.Feedbacks.Update(feedback);
            await _context.SaveChangesAsync();
            return feedback;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var feedback = await GetByIdAsync(id);
            if (feedback == null) return false;

            _context.Feedbacks.Remove(feedback);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> HasCustomerAlreadyFeedback(int accountId)
        {
            return await _context.Feedbacks.AnyAsync(f => f.AccountId == accountId);
        }

        // New required methods
        public async Task<double> getAverageFeedback()
        {
            var feedbacks = await _context.Feedbacks.ToListAsync();
            return feedbacks.Any() ? feedbacks.Average(f => f.Rate) : 0;
        }

        public async Task<List<Feedback>?> getFeedbackByFilter(FilterFeedbackDTO filterFeedbackDTO)
        {
            var query = _context.Feedbacks
                .Include(f => f.Account)
                .ThenInclude(a => a.Customer)
                .Include(f => f.Account)
                .ThenInclude(a => a.Admin)
                .AsQueryable();

            if (filterFeedbackDTO.Rate > 0)
            {
                query = query.Where(f => f.Rate == filterFeedbackDTO.Rate);
            }

            return await query
                .OrderByDescending(f => f.CreateAt)
                .Skip((filterFeedbackDTO.Page - 1) * filterFeedbackDTO.PageSize)
                .Take(filterFeedbackDTO.PageSize)
                .ToListAsync();
        }

        public async Task<List<Feedback>> GetFeedbacksByAccountIdAsync(int accountId)
        {
            return await _context.Feedbacks
                .Include(f => f.Account)
                .ThenInclude(a => a.Customer)
                .Include(f => f.Account)
                .ThenInclude(a => a.Admin)
                .Where(f => f.AccountId == accountId)
                .OrderByDescending(f => f.CreateAt)
                .ToListAsync();
        }

        public async Task<List<Feedback>> GetFeedbacksByTicketIdAsync(int ticketId)
        {
            var feedbackIds = _mappingService.GetFeedbackIdsByTicketId(ticketId);
            
            if (!feedbackIds.Any())
                return new List<Feedback>();

            return await _context.Feedbacks
                .Where(f => feedbackIds.Contains(f.FeedbackId))
                .Include(f => f.Account)
                .ThenInclude(a => a.Customer)
                .Include(f => f.Account)
                .ThenInclude(a => a.Admin)
                .OrderByDescending(f => f.CreateAt)
                .ToListAsync();
        }

        public async Task<List<FeedbackDetailDTO>> GetFeedbacksDetailByTicketIdAsync(int ticketId)
        {
            var feedbackIds = _mappingService.GetFeedbackIdsByTicketId(ticketId);
            
            if (!feedbackIds.Any())
                return new List<FeedbackDetailDTO>();

            var query = from feedback in _context.Feedbacks
                        join account in _context.Accounts on feedback.AccountId equals account.AccountId
                        join ticket in _context.Tickets on ticketId equals ticket.TicketId
                        join flight in _context.Flights on ticket.FlightId equals flight.FlightId
                        join departureAirport in _context.Airports on flight.DepartureAirportId equals departureAirport.AirportId
                        join arrivalAirport in _context.Airports on flight.ArrivalAirportId equals arrivalAirport.AirportId
                        where feedbackIds.Contains(feedback.FeedbackId)
                        select new FeedbackDetailDTO
                        {
                            FeedbackId = feedback.FeedbackId,
                            Title = feedback.Title,
                            Rate = feedback.Rate,
                            Content = feedback.Content,
                            CreateAt = feedback.CreateAt,
                            AccountId = account.AccountId,
                            AccountName = account.Username,
                            TicketId = ticket.TicketId,
                            TicketNumber = ticket.TicketNumber,
                            BookingDate = ticket.BookingDate,
                            TotalPrice = ticket.TotalPrice,
                            PassengerName = ticket.FullName,
                            FlightId = flight.FlightId,
                            FlightNumber = flight.FlightCode,
                            DepartureTime = flight.DepartureTime,
                            ArrivalTime = flight.ArrivalTime,
                            DepartureAirport = $"{departureAirport.City} ({departureAirport.AirportCode})",
                            ArrivalAirport = $"{arrivalAirport.City} ({arrivalAirport.AirportCode})",
                            FlightDate = DateOnly.FromDateTime(flight.DepartureTime)
                        };

            return await query.OrderByDescending(f => f.CreateAt).ToListAsync();
        }

        public async Task<List<FeedbackDetailDTO>> GetFeedbacksDetailByAccountIdAsync(int accountId)
        {
            // Lấy tất cả feedbacks của account này
            var feedbacks = await _context.Feedbacks
                .Where(f => f.AccountId == accountId)
                .ToListAsync();

            var result = new List<FeedbackDetailDTO>();

            foreach (var feedback in feedbacks)
            {
                var ticketId = _mappingService.GetTicketIdByFeedbackId(feedback.FeedbackId);
                if (ticketId.HasValue)
                {
                    var detail = await GetFeedbackDetailByIdAndTicketId(feedback.FeedbackId, ticketId.Value);
                    if (detail != null)
                        result.Add(detail);
                }
            }

            return result.OrderByDescending(f => f.CreateAt).ToList();
        }

        private async Task<FeedbackDetailDTO?> GetFeedbackDetailByIdAndTicketId(int feedbackId, int ticketId)
        {
            var query = from feedback in _context.Feedbacks
                        join account in _context.Accounts on feedback.AccountId equals account.AccountId
                        join ticket in _context.Tickets on ticketId equals ticket.TicketId
                        join flight in _context.Flights on ticket.FlightId equals flight.FlightId
                        join departureAirport in _context.Airports on flight.DepartureAirportId equals departureAirport.AirportId
                        join arrivalAirport in _context.Airports on flight.ArrivalAirportId equals arrivalAirport.AirportId
                        where feedback.FeedbackId == feedbackId
                        select new FeedbackDetailDTO
                        {
                            FeedbackId = feedback.FeedbackId,
                            Title = feedback.Title,
                            Rate = feedback.Rate,
                            Content = feedback.Content,
                            CreateAt = feedback.CreateAt,
                            AccountId = account.AccountId,
                            AccountName = account.Username,
                            TicketId = ticket.TicketId,
                            TicketNumber = ticket.TicketNumber,
                            BookingDate = ticket.BookingDate,
                            TotalPrice = ticket.TotalPrice,
                            PassengerName = ticket.FullName,
                            FlightId = flight.FlightId,
                            FlightNumber = flight.FlightCode,
                            DepartureTime = flight.DepartureTime,
                            ArrivalTime = flight.ArrivalTime,
                            DepartureAirport = $"{departureAirport.City} ({departureAirport.AirportCode})",
                            ArrivalAirport = $"{arrivalAirport.City} ({arrivalAirport.AirportCode})",
                            FlightDate = DateOnly.FromDateTime(flight.DepartureTime)
                        };

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<FeedbackDetailDTO>> GetAllFeedbackDetailsAsync()
        {
            // Lấy tất cả feedbacks có mapping với ticket
            var allFeedbacks = await _context.Feedbacks.ToListAsync();
            var result = new List<FeedbackDetailDTO>();

            foreach (var feedback in allFeedbacks)
            {
                var ticketId = _mappingService.GetTicketIdByFeedbackId(feedback.FeedbackId);
                if (ticketId.HasValue)
                {
                    var detail = await GetFeedbackDetailByIdAndTicketId(feedback.FeedbackId, ticketId.Value);
                    if (detail != null)
                        result.Add(detail);
                }
            }

            return result.OrderByDescending(f => f.CreateAt).ToList();
        }

        public async Task<List<FeedbackDetailDTO>> SearchAndFilterFeedbacksAsync(string? searchTerm = null, int? rating = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            // Lấy tất cả feedbacks và áp dụng filter
            var query = _context.Feedbacks.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Tìm kiếm theo: Tiêu đề feedback, Tên khách hàng, ID chuyến bay
                query = query.Where(f => 
                    f.Title.Contains(searchTerm) || 
                    (f.Account != null && f.Account.AccountName.Contains(searchTerm)) ||
                    _context.Tickets
                        .Where(t => t.TicketId == _context.FeedbackTickets
                            .Where(ft => ft.FeedbackId == f.FeedbackId)
                            .Select(ft => ft.TicketId)
                            .FirstOrDefault())
                        .Any(t => t.Flight.FlightNumber.Contains(searchTerm))
                );
            }

            if (rating.HasValue)
            {
                query = query.Where(f => f.Rate == rating.Value);
            }

            if (fromDate.HasValue)
            {
                var fromDateOnly = DateOnly.FromDateTime(fromDate.Value);
                query = query.Where(f => f.CreateAt >= fromDateOnly);
            }

            if (toDate.HasValue)
            {
                var toDateOnly = DateOnly.FromDateTime(toDate.Value);
                query = query.Where(f => f.CreateAt <= toDateOnly);
            }

            var feedbacks = await query.ToListAsync();
            var result = new List<FeedbackDetailDTO>();

            foreach (var feedback in feedbacks)
            {
                var ticketId = _mappingService.GetTicketIdByFeedbackId(feedback.FeedbackId);
                if (ticketId.HasValue)
                {
                    var detail = await GetFeedbackDetailByIdAndTicketId(feedback.FeedbackId, ticketId.Value);
                    if (detail != null)
                        result.Add(detail);
                }
            }

            return result.OrderByDescending(f => f.CreateAt).ToList();
        }

        public async Task<List<FeedbackDetailDTO>> GetFeedbackDetailsByTicketIdAndAccountIdAsync(int ticketId, int accountId)
        {
            // Get feedbacks for this ticket using mapping service
            var feedbackIds = _mappingService.GetFeedbackIdsByTicketId(ticketId);
            
            var result = new List<FeedbackDetailDTO>();
            
            foreach (var feedbackId in feedbackIds)
            {
                var query = from feedback in _context.Feedbacks
                            join account in _context.Accounts on feedback.AccountId equals account.AccountId
                            join ticket in _context.Tickets on ticketId equals ticket.TicketId
                            join flight in _context.Flights on ticket.FlightId equals flight.FlightId
                            join departureAirport in _context.Airports on flight.DepartureAirportId equals departureAirport.AirportId
                            join arrivalAirport in _context.Airports on flight.ArrivalAirportId equals arrivalAirport.AirportId
                            where feedback.FeedbackId == feedbackId && feedback.AccountId == accountId
                            select new FeedbackDetailDTO
                            {
                                FeedbackId = feedback.FeedbackId,
                                Title = feedback.Title,
                                Rate = feedback.Rate,
                                Content = feedback.Content,
                                CreateAt = feedback.CreateAt,
                                AccountId = account.AccountId,
                                AccountName = account.Username,
                                TicketId = ticket.TicketId,
                                TicketNumber = ticket.TicketNumber,
                                BookingDate = ticket.BookingDate,
                                TotalPrice = ticket.TotalPrice,
                                PassengerName = ticket.FullName,
                                FlightId = flight.FlightId,
                                FlightNumber = flight.FlightCode,
                                DepartureTime = flight.DepartureTime,
                                ArrivalTime = flight.ArrivalTime,
                                DepartureAirport = $"{departureAirport.City} ({departureAirport.AirportCode})",
                                ArrivalAirport = $"{arrivalAirport.City} ({arrivalAirport.AirportCode})",
                                FlightDate = DateOnly.FromDateTime(flight.DepartureTime)
                            };

                var details = await query.ToListAsync();
                result.AddRange(details);
            }

            return result.OrderByDescending(f => f.CreateAt).ToList();
        }
    }
}