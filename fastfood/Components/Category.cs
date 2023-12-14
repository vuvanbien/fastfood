using fastfood.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fastfood.Components
{
    [ViewComponent(Name = "Category")]
    public class CategoryComponent : ViewComponent
    {
        private readonly FoodshopContext _context;

        public CategoryComponent(FoodshopContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }
    }
}