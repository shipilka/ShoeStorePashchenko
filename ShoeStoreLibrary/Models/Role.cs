using System;
using System.Collections.Generic;

namespace ShoeStoreLibrary.Models;

public partial class Role
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public virtual ICollection<UserRoles> UserRoles { get; set; } = new List<UserRoles>(); // связь с UserRoles}
}
