using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeStoreLibrary.DTOs
{
    public class UpdateOrderDto
    {
        [Range(1, 2, ErrorMessage = "Допустимые значения: 1 - Завершен, 2 - Новый")]
        public int OrderStatusId { get; set; }
        public DateOnly DeliveryDate { get; set; }
    }
}
