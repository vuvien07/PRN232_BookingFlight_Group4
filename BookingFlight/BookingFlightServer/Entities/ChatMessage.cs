using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entities;

public partial class ChatMessage
{
    public int MessageId { get; set; }

    public int CustomerId { get; set; }

    public int? SupporterId { get; set; }

    public string SenderType { get; set; } = null!;

    public string MessageContent { get; set; } = null!;

    public DateTime SentAt { get; set; }

    public bool IsRead { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Supporter? Supporter { get; set; }
}
