using BookingFlightServer.Services;
using BookingFlightServer.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Services.Implements
{
    public class FeedbackTicketMappingService : IFeedbackTicketMappingService
    {
        private readonly BookingFlightContext _context;

        public FeedbackTicketMappingService(BookingFlightContext context)
        {
            _context = context;
        }

        // Get feedback IDs by ticket ID using content pattern [TICKET:ticketId]
        public async Task<List<int>> GetFeedbackIdsByTicketId(int ticketId)
        {
            var feedbacks = await _context.Feedbacks
                .Where(f => f.Content != null && f.Content.Contains($"[TICKET:{ticketId}]"))
                .Select(f => f.FeedbackId)
                .ToListAsync();
            
            return feedbacks;
        }

        // Get ticket ID by feedback ID using content pattern [TICKET:ticketId]
        public async Task<int?> GetTicketIdByFeedbackId(int feedbackId)
        {
            var feedback = await _context.Feedbacks
                .Where(f => f.FeedbackId == feedbackId && f.Content != null)
                .FirstOrDefaultAsync();

            if (feedback?.Content != null)
            {
                // Extract ticket ID from pattern [TICKET:123]
                var startPattern = "[TICKET:";
                var endPattern = "]";
                var startIndex = feedback.Content.IndexOf(startPattern);
                
                if (startIndex != -1)
                {
                    startIndex += startPattern.Length;
                    var endIndex = feedback.Content.IndexOf(endPattern, startIndex);
                    
                    if (endIndex != -1)
                    {
                        var ticketIdStr = feedback.Content.Substring(startIndex, endIndex - startIndex);
                        if (int.TryParse(ticketIdStr, out int ticketId))
                        {
                            return ticketId;
                        }
                    }
                }
            }

            return null;
        }

        // Check if feedback exists for ticket and account
        public async Task<bool> HasFeedbackForTicketAndAccount(int ticketId, int accountId)
        {
            var exists = await _context.Feedbacks
                .Where(f => f.AccountId == accountId && 
                           f.Content != null && 
                           f.Content.Contains($"[TICKET:{ticketId}]"))
                .AnyAsync();
            
            return exists;
        }

        // Link feedback to ticket by embedding ticket ID in content (already done during creation)
        public async Task<bool> LinkFeedbackToTicketAndAccount(int feedbackId, int ticketId, int accountId)
        {
            // This is already handled during feedback creation by embedding [TICKET:ticketId] in content
            // So we just return true as linking is done via content pattern
            return await Task.FromResult(true);
        }
    }
}