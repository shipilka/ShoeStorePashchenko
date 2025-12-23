using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.Models;

namespace ShoeStoreWebApplication.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private readonly ShoeStoreLibrary.Contexts.ShoeStoreContext _context;

        public CreateModel(ShoeStoreLibrary.Contexts.ShoeStoreContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["FullNameId"] = new SelectList(_context.FullNames, "FullNameId", "FirstName");
        ViewData["OrderStatusId"] = new SelectList(_context.OrderStatuses, "OrderStatusId", "OrderStatusName");
            return Page();
        }

        [BindProperty]
        public Order Order { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
