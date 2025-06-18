using System;
using System.Collections.Generic;

namespace BookingFlightClient.Entities;

public partial class Admin
{
    public int AdminId { get; set; }

    public string Fullname { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;
}
