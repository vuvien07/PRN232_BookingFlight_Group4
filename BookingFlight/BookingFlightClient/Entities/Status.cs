using System;
using System.Collections.Generic;

namespace BookingFlightClient.Entities;

public partial class Status
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public string StatusType { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Plane> Planes { get; set; } = new List<Plane>();

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
