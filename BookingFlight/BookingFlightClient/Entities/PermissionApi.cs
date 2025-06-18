using System;
using System.Collections.Generic;

namespace BookingFlightClient.Entities;

public partial class PermissionApi
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Url { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
