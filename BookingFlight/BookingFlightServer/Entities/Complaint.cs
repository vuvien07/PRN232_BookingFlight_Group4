using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entities;

public partial class Complaint
{
    public int ComplaintId { get; set; }

    public int StatusId { get; set; }

    public int SupporterId { get; set; }

    public int CustomerId { get; set; }

    public DateTime? CreateAt { get; set; }

    public string Description { get; set; } = null!;

    public string? FileType { get; set; }

    public string? FileUrl { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;

    public virtual Supporter Supporter { get; set; } = null!;
}
