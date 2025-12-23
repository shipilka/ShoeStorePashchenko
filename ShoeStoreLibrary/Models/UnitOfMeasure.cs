using System;
using System.Collections.Generic;

namespace ShoeStoreLibrary.Models;

public partial class UnitOfMeasure
{
    public int UnitOfMeasureId { get; set; }

    public string UnitName { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
