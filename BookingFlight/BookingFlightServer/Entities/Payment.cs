using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int Price { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public int TicketId { get; set; }

    public DateOnly PaymentDate { get; set; }

    public int StatusId { get; set; }

    public virtual Status Status { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
