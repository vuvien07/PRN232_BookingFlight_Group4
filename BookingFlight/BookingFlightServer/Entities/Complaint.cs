using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingFlightServer.Entities;

public partial class Complaint
{
    [Column("complaint_id")]
    public int ComplaintId { get; set; }

    [Column("status_id")]
    public int StatusId { get; set; }

    [Column("supporter_id")]
    public int SupporterId { get; set; }

    [Column("customer_id")]
    public int CustomerId { get; set; }

    [Column("create_at")]
    public DateTime? CreateAt { get; set; }

    [Column("description")]
    public string Description { get; set; } = null!;

    [Column("file_type")]
    public string? FileType { get; set; }

    [Column("file_url")]
    public string? FileUrl { get; set; }

    // public string? FileName { get; set; } // Column doesn't exist in DB

    public virtual Customer Customer { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;

    public virtual Supporter Supporter { get; set; } = null!;
}
