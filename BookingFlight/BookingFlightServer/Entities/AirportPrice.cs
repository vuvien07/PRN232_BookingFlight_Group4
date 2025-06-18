using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entities;

public partial class AirportPrice
{
    public int AirportPriceId { get; set; }

    public int AirportFromId { get; set; }

    public int AirportToId { get; set; }

    public decimal BasePrice { get; set; }

    public virtual Airport AirportFrom { get; set; } = null!;

    public virtual Airport AirportTo { get; set; } = null!;
}
