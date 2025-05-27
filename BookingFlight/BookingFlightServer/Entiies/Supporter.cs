using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entiies;

public partial class Supporter
{
    public int SupporterId { get; set; }

    public string Fullname { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
}
