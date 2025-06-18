using System;
using System.Collections.Generic;

namespace BookingFlightClient.Entities;

public partial class ClassSeat
{
    public int ClassId { get; set; }

    public string ClassName { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
