using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.DTOs;
using ShoeStoreLibrary.Services;

namespace ShoeStoreWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // Получить заказы текущего пользователя (доступ только авторизованным)
        [HttpGet("user")]
        [Authorize]
        public IActionResult GetOrdersByUser()
        {
            var userLogin = User.Identity.Name;

            if (User.IsInRole("Администратор") || User.IsInRole("Менеджер"))
            {
                // Возвращаем все заказы
                var orders = _orderService.GetAllOrders();
                return Ok(orders);
            }
            else
            {
                var orders = _orderService.GetOrdersByUser(userLogin);
                return Ok(orders);
            }
        }

        // Получить заказы по логину (доступ только для администраторов и менеджеров)
        [HttpGet("user/{login}")]
        [Authorize(Roles = "Администратор,Менеджер")]
        public IActionResult GetOrdersByUserLogin(string login)
        {
            var orders = _orderService.GetOrdersByUserLogin(login);
            return Ok(orders);
        }

        // Обновление статуса и даты доставки заказа (доступ для администраторов и менеджеров)
        [HttpPut("{orderId}")]
        [Authorize(Roles = "Администратор,Менеджер")]
        public IActionResult UpdateOrder(int orderId, [FromBody] UpdateOrderDto updateOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _orderService.UpdateOrder(orderId, updateOrderDto);
            return NoContent();
        }
    }
}