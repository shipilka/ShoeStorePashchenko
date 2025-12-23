using System;
using System.Collections.Generic;

namespace ShoeStoreLibrary.Models;

public partial class User
{
    public int UserId { get; set; }
    public int FullNameId { get; set; }
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public virtual FullName FullName { get; set; } = null!;
    public virtual ICollection<Order> Orders { get; set; }
    public virtual ICollection<UserRoles> UserRoles { get; set; } = new List<UserRoles>();
    
}
