using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entiies;

public partial class News
{
    public int NewId { get; set; }

    public string Title { get; set; } = null!;

    public string? Image { get; set; }

    public string Content { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string Author { get; set; } = null!;

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;
}
