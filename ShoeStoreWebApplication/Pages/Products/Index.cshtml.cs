using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.Models;

namespace ShoeStoreWebApplication.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly ShoeStoreLibrary.Contexts.ShoeStoreContext _context;

        public IndexModel(ShoeStoreLibrary.Contexts.ShoeStoreContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get; set; } = default!;
        public IList<Manufacturer> Manufacturers { get; set; } = new List<Manufacturer>();

        public async Task OnGetAsync(string? searchTerm, string? selectedManufacturer, decimal? maxPrice,
                              bool? showDiscounted, bool? inStock, string sortBy)
        {
            // Загружаем производителей для выпадающего списка
            Manufacturers = await _context.Manufacturers.ToListAsync();

            // Получаем все продукты из базы данных
            var productsQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.ProductName)
                .Include(p => p.Supplier)
                .Include(p => p.UnitOfMeasure)
                .AsQueryable();

            // Фильтрация по поисковому термину
            if (!string.IsNullOrEmpty(searchTerm))
            {
                productsQuery = productsQuery.Where(p => p.ProductDescription.Contains(searchTerm));
            }

            // Фильтрация по производителю
            if (!string.IsNullOrEmpty(selectedManufacturer))
            {
                productsQuery = productsQuery.Where(p => p.Manufacturer.ManufacturerName == selectedManufacturer);
            }

            // Фильтрация по максимальной цене
            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
            }

            // Фильтрация по наличию скидки
            if (showDiscounted.HasValue && showDiscounted.Value)
            {
                productsQuery = productsQuery.Where(p => p.ActiveDiscount > 0);
            }

            // Фильтрация по наличию на складе
            if (inStock.HasValue && inStock.Value)
            {
                productsQuery = productsQuery.Where(p => p.StockQuantity > 0);
            }

            // Сортировка
            switch (sortBy)
            {
                case "Name":
                    productsQuery = productsQuery.OrderBy(p => p.ProductName.ProductName);
                    break;
                case "Manufacturer":
                    productsQuery = productsQuery.OrderBy(p => p.Manufacturer.ManufacturerName);
                    break;
                case "Price":
                    productsQuery = productsQuery.OrderBy(p => p.Price);
                    break;
                case "PriceDesc":
                    productsQuery = productsQuery.OrderByDescending(p => p.Price);
                    break;
            }

            // Загрузка отфильтрованных и отсортированных товаров
            Product = await productsQuery.ToListAsync();
        }

        public IActionResult OnGetImage(string productId)
        {
            // Поиск продукта по идентификатору
            var product = _context.Products.Find(productId);

            // Проверка наличия продукта и его изображения
            if (product?.Photo != null)
            {
                if (product.Photo.Length > 0)
                {
                    // Возврат изображения из базы данных
                    return File(product.Photo, "image/jpeg");
                }
            }

            // Возврат заглушки, если изображение отсутствует
            return File("~/images/picture.png", "image/png");
        }

        public async Task<IActionResult> OnPostCreateOrderAsync(string productId) // productId - ID товара
        {
            // Получаем ID текущего пользователя из сессии
            var userId = HttpContext.Session.GetInt32("UserId");

            // Проверяем, авторизован ли пользователь
            if (!userId.HasValue)
            {
                return RedirectToPage("/Login"); // Если не авторизован, перенаправляем на страницу логина
            }

            // Поиск товара по его ID
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return BadRequest("Товар не найден."); // Обработка случая, если товар не найден
            }

            // Получение статуса "Новый" из базы данных
            var orderStatus = await _context.OrderStatuses.FirstOrDefaultAsync(os => os.OrderStatusName == "Новый");
            if (orderStatus == null)
            {
                return BadRequest("Статус заказа 'Новый' не найден."); // Обработка случая, если статус не найден
            }

            // Создание нового заказа
            var newOrder = new Order
            {
                UserId = userId.Value, // ID пользователя
                OrderDate = DateOnly.FromDateTime(DateTime.Now), // Текущая дата
                DeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)), // Дата доставки через неделю
                PickupCode = new Random().Next(100, 999), // Случайный код для получения заказа
                OrderStatusId = orderStatus.OrderStatusId // Используем ID статуса заказа
            };

            // Создание детали заказа
            var orderDetail = new OrderDetail
            {
                ProductId = productId, // ID товара
                Quantity = 1 // Количество товара в заказе
            };

            // Добавляем деталь заказа в новый заказ
            newOrder.OrderDetails.Add(orderDetail);

            // Добавляем новый заказ в контекст и сохраняем
            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            // Перенаправляем на страницу отображения заказов
            return RedirectToPage("/Orders/Index");
        }
    }
}