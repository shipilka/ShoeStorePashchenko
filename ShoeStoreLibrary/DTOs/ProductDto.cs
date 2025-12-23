using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeStoreLibrary.DTOs
{
    public class ProductDto
    {
        public string? ProductId { get; set; }
        public int ProductNameId { get; set; }
        public decimal Price { get; set; }
        public int SupplierId { get; set; }
        public int ManufacturerId { get; set; }
        public int CategoryId { get; set; }
        public int ActiveDiscount { get; set; }
        public int StockQuantity { get; set; }
        public int UnitOfMeasureId { get; set; }
        public string? ProductDescription { get; set; }
    }
}
