using BookingFlightServer.Entities;
using BookingFlightServer.DTO.Request;
using BookingFlightServer.DTO.Response;
using BookingFlightServer.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly BookingFlightContext _context;

        public FeedbackRepository(BookingFlightContext context)
        {
            _context = context;
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
            return await _context.FeedbackTickets
                .Where(ft => ft.TicketId == ticketId)
                .Include(ft => ft.Feedback)
                .ThenInclude(f => f.Account)
                .ThenInclude(a => a.Customer)
                .Include(ft => ft.Feedback)
                .ThenInclude(f => f.Account)
                .ThenInclude(a => a.Admin)
                .Select(ft => ft.Feedback)
                .OrderByDescending(f => f.CreateAt)
                .ToListAsync();
        }

        public async Task<List<FeedbackDetailDTO>> GetFeedbacksDetailByTicketIdAsync(int ticketId)
        {
            var query = from feedbackTicket in _context.FeedbackTickets
                        join feedback in _context.Feedbacks on feedbackTicket.FeedbackId equals feedback.FeedbackId
                        join account in _context.Accounts on feedback.AccountId equals account.AccountId
                        join ticket in _context.Tickets on feedbackTicket.TicketId equals ticket.TicketId
                        join flight in _context.Flights on ticket.FlightId equals flight.FlightId
                        join departureAirport in _context.Airports on flight.DepartureAirportId equals departureAirport.AirportId
                        join arrivalAirport in _context.Airports on flight.ArrivalAirportId equals arrivalAirport.AirportId
                        where feedbackTicket.TicketId == ticketId
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
            var query = from feedbackTicket in _context.FeedbackTickets
                        join feedback in _context.Feedbacks on feedbackTicket.FeedbackId equals feedback.FeedbackId
                        join account in _context.Accounts on feedback.AccountId equals account.AccountId
                        join ticket in _context.Tickets on feedbackTicket.TicketId equals ticket.TicketId
                        join flight in _context.Flights on ticket.FlightId equals flight.FlightId
                        join departureAirport in _context.Airports on flight.DepartureAirportId equals departureAirport.AirportId
                        join arrivalAirport in _context.Airports on flight.ArrivalAirportId equals arrivalAirport.AirportId
                        where feedback.AccountId == accountId
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

        public async Task<List<FeedbackDetailDTO>> GetAllFeedbackDetailsAsync()
        {
            var query = from feedbackTicket in _context.FeedbackTickets
                        join feedback in _context.Feedbacks on feedbackTicket.FeedbackId equals feedback.FeedbackId
                        join account in _context.Accounts on feedback.AccountId equals account.AccountId
                        join ticket in _context.Tickets on feedbackTicket.TicketId equals ticket.TicketId
                        join flight in _context.Flights on ticket.FlightId equals flight.FlightId
                        join departureAirport in _context.Airports on flight.DepartureAirportId equals departureAirport.AirportId
                        join arrivalAirport in _context.Airports on flight.ArrivalAirportId equals arrivalAirport.AirportId
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

        public async Task<List<FeedbackDetailDTO>> SearchAndFilterFeedbacksAsync(string? searchTerm = null, int? rating = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = from feedbackTicket in _context.FeedbackTickets
                        join feedback in _context.Feedbacks on feedbackTicket.FeedbackId equals feedback.FeedbackId
                        join account in _context.Accounts on feedback.AccountId equals account.AccountId
                        join ticket in _context.Tickets on feedbackTicket.TicketId equals ticket.TicketId
                        join flight in _context.Flights on ticket.FlightId equals flight.FlightId
                        join departureAirport in _context.Airports on flight.DepartureAirportId equals departureAirport.AirportId
                        join arrivalAirport in _context.Airports on flight.ArrivalAirportId equals arrivalAirport.AirportId
                        select new
                        {
                            feedback,
                            account,
                            ticket,
                            flight,
                            departureAirport,
                            arrivalAirport
                        };

            // Apply filters
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => x.feedback.Title.Contains(searchTerm) || 
                                        x.feedback.Content.Contains(searchTerm) ||
                                        x.account.Username.Contains(searchTerm));
            }

            if (rating.HasValue)
            {
                query = query.Where(x => x.feedback.Rate == rating.Value);
            }

            if (fromDate.HasValue)
            {
                var fromDateOnly = DateOnly.FromDateTime(fromDate.Value);
                query = query.Where(x => x.feedback.CreateAt >= fromDateOnly);
            }

            if (toDate.HasValue)
            {
                var toDateOnly = DateOnly.FromDateTime(toDate.Value);
                query = query.Where(x => x.feedback.CreateAt <= toDateOnly);
            }

            var result = await query.Select(x => new FeedbackDetailDTO
            {
                FeedbackId = x.feedback.FeedbackId,
                Title = x.feedback.Title,
                Rate = x.feedback.Rate,
                Content = x.feedback.Content,
                CreateAt = x.feedback.CreateAt,
                AccountId = x.account.AccountId,
                AccountName = x.account.Username,
                TicketId = x.ticket.TicketId,
                TicketNumber = x.ticket.TicketNumber,
                BookingDate = x.ticket.BookingDate,
                TotalPrice = x.ticket.TotalPrice,
                PassengerName = x.ticket.FullName,
                FlightId = x.flight.FlightId,
                FlightNumber = x.flight.FlightCode,
                DepartureTime = x.flight.DepartureTime,
                ArrivalTime = x.flight.ArrivalTime,
                DepartureAirport = $"{x.departureAirport.City} ({x.departureAirport.AirportCode})",
                ArrivalAirport = $"{x.arrivalAirport.City} ({x.arrivalAirport.AirportCode})",
                FlightDate = DateOnly.FromDateTime(x.flight.DepartureTime)
            }).OrderByDescending(f => f.CreateAt).ToListAsync();

            return result;
        }

        public async Task<List<FeedbackDetailDTO>> GetFeedbackDetailsByTicketIdAndAccountIdAsync(int ticketId, int accountId)
        {
            var query = from feedbackTicket in _context.FeedbackTickets
                        join feedback in _context.Feedbacks on feedbackTicket.FeedbackId equals feedback.FeedbackId
                        join account in _context.Accounts on feedback.AccountId equals account.AccountId
                        join ticket in _context.Tickets on feedbackTicket.TicketId equals ticket.TicketId
                        join flight in _context.Flights on ticket.FlightId equals flight.FlightId
                        join departureAirport in _context.Airports on flight.DepartureAirportId equals departureAirport.AirportId
                        join arrivalAirport in _context.Airports on flight.ArrivalAirportId equals arrivalAirport.AirportId
                        where feedbackTicket.TicketId == ticketId && feedback.AccountId == accountId
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

        public async Task<bool> HasCustomerAlreadyFeedback(int accountId)
        {
            return await _context.Feedbacks
                .AnyAsync(f => f.AccountId == accountId);
        }
    }
}
