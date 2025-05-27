using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entiies;

public partial class Seat
{
    public int SeatId { get; set; }

    public string SeatNumber { get; set; } = null!;

    public int ClassId { get; set; }

    public int StatusId { get; set; }

    public int? PlaneId { get; set; }

    public virtual ClassSeat Class { get; set; } = null!;

    public virtual ICollection<FlightSeat> FlightSeats { get; set; } = new List<FlightSeat>();

    public virtual Plane? Plane { get; set; }

    public virtual Status Status { get; set; } = null!;
}
