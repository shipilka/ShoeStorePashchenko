using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeStoreLibrary.DTOs
{
    public class OrderDetailDto
    {
        public int OrderDetailId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
