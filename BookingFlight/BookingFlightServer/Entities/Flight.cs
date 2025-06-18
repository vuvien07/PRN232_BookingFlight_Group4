using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entities;

public partial class Flight
{
    public int FlightId { get; set; }

    public string FlightCode { get; set; } = null!;

    public decimal Tax { get; set; }

    public int StatusId { get; set; }

    public DateTime DepartureTime { get; set; }

    public DateTime ArrivalTime { get; set; }

    public int PlaneId { get; set; }

    public int ManagerId { get; set; }

    public int CustomerId { get; set; }

    public int DepartureAirportId { get; set; }

    public int ArrivalAirportId { get; set; }

    public virtual Airport ArrivalAirport { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual Airport DepartureAirport { get; set; } = null!;

    public virtual ICollection<FlightSeat> FlightSeats { get; set; } = new List<FlightSeat>();

    public virtual Manager Manager { get; set; } = null!;

    public virtual Plane Plane { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
