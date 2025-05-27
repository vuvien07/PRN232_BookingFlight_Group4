using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entiies;

public partial class Service
{
    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public string? Detail { get; set; }

    public int ManagerId { get; set; }

    public int? StatusId { get; set; }

    public virtual Manager Manager { get; set; } = null!;

    public virtual Status? Status { get; set; }

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
