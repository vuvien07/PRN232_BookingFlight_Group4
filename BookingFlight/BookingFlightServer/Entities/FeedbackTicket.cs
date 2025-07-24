using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entities;

public partial class FeedbackTicket
{
    public int Id { get; set; }
    public int FeedbackId { get; set; }
    public int TicketId { get; set; }
    public DateOnly? CreatedAt { get; set; }

    public virtual Feedback Feedback { get; set; } = null!;
    public virtual Ticket Ticket { get; set; } = null!;
}
