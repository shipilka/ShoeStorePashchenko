using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoeStoreWebApplication.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ShoeStoreLibrary.Contexts.ShoeStoreContext _context;
        [BindProperty]
        public string Login { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public LoginModel(ShoeStoreContext context)
        {
            _context = context;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync() // Асинхронный метод для логина
        {
            var user = await _context.Users
        .Include(u => u.FullName)
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => u.Login == Login && u.Password == Password);

            if (user != null)
            {
                // Установка сессии
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("FullName", $"{user.FullName.LastName} {user.FullName.FirstName} {user.FullName.Patronymic}");

                // Получение ролей
                var roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();
                HttpContext.Session.SetString("UserRoles", string.Join(",", roles));

                return RedirectToPage("/Index"); // Перенаправление на главную страницу
            }

            // Если неверный логин или пароль
            ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
            return Page();
        }

        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Clear(); // Очистка всей сессии
            return RedirectToPage("/Index"); // Перенаправление на главную страницу после выхода
        }
    }
}