namespace BookingFlightServer.DTO.Chat
{
    public class ChatMessageDto
    {
        public int MessageId { get; set; }
        public int CustomerId { get; set; }
        public int? SupporterId { get; set; }
        public string SenderType { get; set; } = null!;
        public string MessageContent { get; set; } = null!;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public string? SenderName { get; set; }
    }

    public class SendMessageRequest
    {
        public int CustomerId { get; set; }
        public int? SupporterId { get; set; }
        public string SenderType { get; set; } = null!;
        public string MessageContent { get; set; } = null!;
    }

    public class ChatHistoryRequest
    {
        public int CustomerId { get; set; }
        public int? SupporterId { get; set; }
        public int PageSize { get; set; } = 50;
        public int PageNumber { get; set; } = 1;
    }

    public class ChatConversationDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public int? SupporterId { get; set; }
        public string? SupporterName { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
    }
}
