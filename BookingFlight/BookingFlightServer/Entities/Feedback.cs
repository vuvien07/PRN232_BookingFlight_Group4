using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entities;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public string Title { get; set; } = null!;

    public int Rate { get; set; }

    public string Content { get; set; } = null!;

    public DateOnly? CreateAt { get; set; }

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;
}
