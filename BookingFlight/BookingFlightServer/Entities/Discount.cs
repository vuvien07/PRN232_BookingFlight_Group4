using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entities;

public partial class Discount
{
    public int DiscountId { get; set; }

    public string DiscountCode { get; set; } = null!;

    public decimal DiscountPercent { get; set; }

    public string DiscountTitle { get; set; } = null!;

    public string? DiscountInfor { get; set; }

    public byte? Status { get; set; }

    public int CustomerId { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
