using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entities;

public partial class TicketItem
{
    public int TicketId { get; set; }

    public int ItemId { get; set; }

    public int? Quantity { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
