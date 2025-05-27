using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entiies;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string Fullname { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
