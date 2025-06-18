using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entities;

public partial class Manager
{
    public int ManagerId { get; set; }

    public string Fullname { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Airport> Airports { get; set; } = new List<Airport>();

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();

    public virtual ICollection<Plane> Planes { get; set; } = new List<Plane>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
