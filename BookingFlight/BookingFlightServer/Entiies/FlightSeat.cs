using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entiies;

public partial class FlightSeat
{
    public int FlightId { get; set; }

    public int SeatId { get; set; }

    public bool? IsSat { get; set; }

    public int? TicketId { get; set; }

    public virtual Flight Flight { get; set; } = null!;

    public virtual Seat Seat { get; set; } = null!;

    public virtual Ticket? Ticket { get; set; }
}
