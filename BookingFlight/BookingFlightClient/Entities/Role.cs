using System;
using System.Collections.Generic;

namespace BookingFlightClient.Entities;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<PermissionApi> PermissionApis { get; set; } = new List<PermissionApi>();

    public virtual ICollection<PermissionPage> PermissionPages { get; set; } = new List<PermissionPage>();
}
