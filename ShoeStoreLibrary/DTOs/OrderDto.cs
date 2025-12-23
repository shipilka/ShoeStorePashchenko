using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeStoreLibrary.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string FullName { get; set; } // для представления полного имени
        public DateOnly OrderDate { get; set; }
        public DateOnly DeliveryDate { get; set; }
        public int PickupCode { get; set; }
        public int OrderStatusId { get; set; }
        public string OrderStatusName { get; set; } // Добавлено для названия статуса заказа
        public ICollection<OrderDetailDto> OrderDetails { get; set; } // для детализации заказа
    }
}
