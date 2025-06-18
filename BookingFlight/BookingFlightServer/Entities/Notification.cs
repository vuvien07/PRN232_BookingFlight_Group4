using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entities;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int AccountId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime? CreateAt { get; set; }

    public int StatusId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}
