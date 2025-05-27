using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entiies;

public partial class Plane
{
    public int PlaneId { get; set; }

    public string PlaneCode { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int StatusId { get; set; }

    public string Manufacture { get; set; } = null!;

    public int YearOfManufacture { get; set; }

    public int ManagerId { get; set; }

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();

    public virtual Manager Manager { get; set; } = null!;

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual Status Status { get; set; } = null!;
}
