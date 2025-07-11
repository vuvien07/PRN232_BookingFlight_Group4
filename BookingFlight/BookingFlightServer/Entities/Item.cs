﻿using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entities;

public partial class Item
{
    public int ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public string? Detail { get; set; }

    public int Price { get; set; }

    public int? StatusId { get; set; }

    public string? Image { get; set; }

    public virtual Status? Status { get; set; }

    public virtual ICollection<TicketItem> TicketItems { get; set; } = new List<TicketItem>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
