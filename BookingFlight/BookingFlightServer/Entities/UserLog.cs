using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entities;

public partial class UserLog
{
    public int LogId { get; set; }

    public int AccountId { get; set; }

    public string? Detail { get; set; }

    public DateTime AccessTime { get; set; }

    public virtual Account Account { get; set; } = null!;
}
