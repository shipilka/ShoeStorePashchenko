using Microsoft.EntityFrameworkCore;
using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.DTOs;
using ShoeStoreLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeStoreLibrary.Services
{
    public class OrderService
    {
        private readonly ShoeStoreContext _context;

        public OrderService(ShoeStoreContext context)
        {
            _context = context;
        }

        public IEnumerable<OrderDto> GetOrdersByUser(string login)
        {
            var userId = _context.Users
                .Where(u => u.Login == login)
                .Select(u => u.UserId) // Изменено на UserId
                .FirstOrDefault();

            if (userId == 0) return Enumerable.Empty<OrderDto>();

            return _context.Orders
                .Where(o => o.UserId == userId) // Используем UserId вместо FullNameId
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    FullName = $"{o.User.FullName.LastName} {o.User.FullName.FirstName} {o.User.FullName.Patronymic}", // Изменение на User.FullName
                    OrderDate = o.OrderDate,
                    DeliveryDate = o.DeliveryDate,
                    PickupCode = o.PickupCode,
                    OrderStatusId = o.OrderStatusId,
                    OrderStatusName = o.OrderStatus.OrderStatusName,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
                    {
                        OrderDetailId = od.OrderDetailId,
                        ProductId = od.ProductId,
                        Quantity = od.Quantity
                    }).ToList()
                })
                .ToList();
        }

        public IEnumerable<OrderDto> GetAllOrders()
        {
            return _context.Orders
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    FullName = $"{o.User.FullName.LastName} {o.User.FullName.FirstName} {o.User.FullName.Patronymic}", // Изменение на User.FullName
                    OrderDate = o.OrderDate,
                    DeliveryDate = o.DeliveryDate,
                    PickupCode = o.PickupCode,
                    OrderStatusId = o.OrderStatusId,
                    OrderStatusName = o.OrderStatus.OrderStatusName,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
                    {
                        OrderDetailId = od.OrderDetailId,
                        ProductId = od.ProductId,
                        Quantity = od.Quantity
                    }).ToList()
                })
                .ToList();
        }

        public IEnumerable<OrderDto> GetOrdersByUserLogin(string login)
        {
            var userId = _context.Users
                .Where(u => u.Login == login)
                .Select(u => u.UserId) // Изменено на UserId
                .FirstOrDefault();

            if (userId == 0) return Enumerable.Empty<OrderDto>();

            return _context.Orders
                .Where(o => o.UserId == userId) // Используем UserId вместо FullNameId
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    FullName = $"{o.User.FullName.LastName} {o.User.FullName.FirstName} {o.User.FullName.Patronymic}", // Изменение на User.FullName
                    OrderDate = o.OrderDate,
                    DeliveryDate = o.DeliveryDate,
                    PickupCode = o.PickupCode,
                    OrderStatusId = o.OrderStatusId,
                    OrderStatusName = o.OrderStatus.OrderStatusName,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
                    {
                        OrderDetailId = od.OrderDetailId,
                        ProductId = od.ProductId,
                        Quantity = od.Quantity
                    }).ToList()
                })
                .ToList();
        }

        public void UpdateOrder(int orderId, UpdateOrderDto updateOrderDto)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                order.OrderStatusId = updateOrderDto.OrderStatusId;
                order.DeliveryDate = updateOrderDto.DeliveryDate;

                try
                {
                    var result = _context.SaveChanges();
                    if (result > 0)
                    {
                        // Успешно
                    }
                    else
                    {
                        // Изменения не были внесены в базу данных
                        throw new Exception("No changes were made");
                    }
                }
                catch (Exception ex)
                {
                    
                    throw;
                }
            }
            else
            {
                throw new Exception("Order not found");
            }
        }
    }
}