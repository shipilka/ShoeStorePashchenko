using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoeStoreWebApplication.Pages.AllOrders
{
    public class IndexModel : PageModel
    {
        private readonly ShoeStoreContext _context;

        public IndexModel(ShoeStoreContext context)
        {
            _context = context;
        }

        public IList<Order> Orders { get; set; } = new List<Order>();

        public async Task OnGetAsync()
        {
            // ID текущего пользователя из сессии
            var userId = HttpContext.Session.GetInt32("UserId");

            var roles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.RoleName)
                .ToListAsync();

            if (roles.Contains("Администратор") || roles.Contains("Менеджер"))
            {
                Orders = await _context.Orders
                    .Include(o => o.User) // Загрузка информации о пользователе
                        .ThenInclude(u => u.FullName) // Загрузка полного имени пользователя
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .Include(o => o.OrderStatus)
                    .ToListAsync();
            }
            else
            {
                RedirectToPage("/AccessDenied");
            }
        }
    }
}