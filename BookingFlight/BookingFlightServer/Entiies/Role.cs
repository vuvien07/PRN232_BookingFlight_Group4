﻿using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entiies;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
