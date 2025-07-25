using BookingFlightServer.DTO.Chat;

namespace BookingFlightServer.Services
{
    public interface IChatService
    {
        Task<ChatMessageDto> SendMessageAsync(SendMessageRequest request);
        Task<List<ChatMessageDto>> GetChatHistoryAsync(ChatHistoryRequest request);
        Task<List<ChatConversationDto>> GetCustomerConversationsAsync(int customerId);
        Task<List<ChatConversationDto>> GetSupporterConversationsAsync(int supporterId);
        Task<bool> MarkMessageAsReadAsync(int messageId);
        Task<int> GetUnreadMessageCountAsync(int customerId, int? supporterId);
    }
}
