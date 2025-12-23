using System;
using System.Collections.Generic;

namespace ShoeStoreLibrary.Models;

public partial class ProductNames
{
    public int ProductNameId { get; set; }

    public string ProductName { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
