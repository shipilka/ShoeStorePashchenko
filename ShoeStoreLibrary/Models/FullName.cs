using System;
using System.Collections.Generic;

namespace ShoeStoreLibrary.Models;

public partial class FullName
{
    public int FullNameId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? Patronymic { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
