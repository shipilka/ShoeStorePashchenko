using System;
using System.Collections.Generic;

namespace ShoeStoreLibrary.Models;

public partial class Product
{
    public string ProductId { get; set; } = null!;

    public int ProductNameId { get; set; }

    public decimal Price { get; set; }

    public int SupplierId { get; set; }

    public int ManufacturerId { get; set; }

    public int CategoryId { get; set; }

    public int? ActiveDiscount { get; set; }

    public int? StockQuantity { get; set; }

    public int UnitOfMeasureId { get; set; }

    public string? ProductDescription { get; set; }

    public byte[]? Photo { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Manufacturer Manufacturer { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ProductNames ProductName { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual UnitOfMeasure UnitOfMeasure { get; set; } = null!;
}
