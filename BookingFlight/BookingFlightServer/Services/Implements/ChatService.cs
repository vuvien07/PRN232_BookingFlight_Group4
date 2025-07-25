using BookingFlightServer.Data;
using BookingFlightServer.DTO.Chat;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Services.Implements
{
    public class ChatService : IChatService
    {
        private readonly BookingFlightContext _context;

        public ChatService(BookingFlightContext context)
        {
            _context = context;
        }

        public async Task<ChatMessageDto> SendMessageAsync(SendMessageRequest request)
        {
            var message = new ChatMessage
            {
                CustomerId = request.CustomerId,
                SupporterId = request.SupporterId,
                SenderType = request.SenderType,
                MessageContent = request.MessageContent,
                SentAt = DateTime.Now,
                IsRead = false,
                IsDeleted = false
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            // Load related data for response
            var messageWithDetails = await _context.ChatMessages
                .Include(m => m.Customer)
                .Include(m => m.Supporter)
                .FirstOrDefaultAsync(m => m.MessageId == message.MessageId);

            return new ChatMessageDto
            {
                MessageId = messageWithDetails.MessageId,
                CustomerId = messageWithDetails.CustomerId,
                SupporterId = messageWithDetails.SupporterId,
                SenderType = messageWithDetails.SenderType,
                MessageContent = messageWithDetails.MessageContent,
                SentAt = messageWithDetails.SentAt,
                IsRead = messageWithDetails.IsRead,
                SenderName = messageWithDetails.SenderType == "customer" 
                    ? messageWithDetails.Customer.Fullname 
                    : messageWithDetails.Supporter?.Fullname
            };
        }

        public async Task<List<ChatMessageDto>> GetChatHistoryAsync(ChatHistoryRequest request)
        {
            var query = _context.ChatMessages
                .Include(m => m.Customer)
                .Include(m => m.Supporter)
                .Where(m => !m.IsDeleted && m.CustomerId == request.CustomerId);

            if (request.SupporterId.HasValue)
            {
                query = query.Where(m => m.SupporterId == request.SupporterId);
            }

            var messages = await query
                .OrderByDescending(m => m.SentAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(m => new ChatMessageDto
                {
                    MessageId = m.MessageId,
                    CustomerId = m.CustomerId,
                    SupporterId = m.SupporterId,
                    SenderType = m.SenderType,
                    MessageContent = m.MessageContent,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead,
                    SenderName = m.SenderType == "customer" ? m.Customer.Fullname : m.Supporter.Fullname
                })
                .ToListAsync();

            return messages.OrderBy(m => m.SentAt).ToList();
        }

        public async Task<List<ChatConversationDto>> GetCustomerConversationsAsync(int customerId)
        {
            var conversations = await _context.ChatMessages
                .Include(m => m.Customer)
                .Include(m => m.Supporter)
                .Where(m => !m.IsDeleted && m.CustomerId == customerId)
                .GroupBy(m => new { m.CustomerId, m.SupporterId })
                .Select(g => new ChatConversationDto
                {
                    CustomerId = g.Key.CustomerId,
                    CustomerName = g.First().Customer.Fullname,
                    SupporterId = g.Key.SupporterId,
                    SupporterName = g.First().Supporter != null ? g.First().Supporter.Fullname : null,
                    LastMessage = g.OrderByDescending(m => m.SentAt).First().MessageContent,
                    LastMessageTime = g.OrderByDescending(m => m.SentAt).First().SentAt,
                    UnreadCount = g.Count(m => !m.IsRead && m.SenderType == "supporter")
                })
                .OrderByDescending(c => c.LastMessageTime)
                .ToListAsync();

            return conversations;
        }

        public async Task<List<ChatConversationDto>> GetSupporterConversationsAsync(int supporterId)
        {
            var conversations = await _context.ChatMessages
                .Include(m => m.Customer)
                .Include(m => m.Supporter)
                .Where(m => !m.IsDeleted && m.SupporterId == supporterId)
                .GroupBy(m => new { m.CustomerId, m.SupporterId })
                .Select(g => new ChatConversationDto
                {
                    CustomerId = g.Key.CustomerId,
                    CustomerName = g.First().Customer.Fullname,
                    SupporterId = g.Key.SupporterId,
                    SupporterName = g.First().Supporter.Fullname,
                    LastMessage = g.OrderByDescending(m => m.SentAt).First().MessageContent,
                    LastMessageTime = g.OrderByDescending(m => m.SentAt).First().SentAt,
                    UnreadCount = g.Count(m => !m.IsRead && m.SenderType == "customer")
                })
                .OrderByDescending(c => c.LastMessageTime)
                .ToListAsync();

            return conversations;
        }

        public async Task<bool> MarkMessageAsReadAsync(int messageId)
        {
            var message = await _context.ChatMessages.FindAsync(messageId);
            if (message == null) return false;

            message.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetUnreadMessageCountAsync(int customerId, int? supporterId)
        {
            var query = _context.ChatMessages
                .Where(m => !m.IsDeleted && !m.IsRead && m.CustomerId == customerId);

            if (supporterId.HasValue)
            {
                query = query.Where(m => m.SupporterId == supporterId && m.SenderType == "supporter");
            }
            else
            {
                query = query.Where(m => m.SenderType == "supporter");
            }

            return await query.CountAsync();
        }
    }
}
