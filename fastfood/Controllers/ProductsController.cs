using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fastfood.Models;
using fastfood.Helpper;
using X.PagedList;
using fastfood.Models.ViewModel;

namespace fastfood.Controllers
{
    public class ProductsController : Controller
    {
        private readonly FoodshopContext _context;
       

        public ProductsController(FoodshopContext context)
        {
            _context = context;
        }

        // GET: Products
        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            page = page < 1 ? 1 : page;
            int pageSize = 9;

            var productsPagedList =_context.Products.ToPagedList(page, pageSize);

            return View(productsPagedList);
        }
        [HttpPost]
        public IActionResult Search(string keywords, int page = 1)
        {
            int pageSize = 2;

            var productsPagedList = _context.Products
                                            .Include(p => p.Cate)
                                            .Where(p => p.ProName.Contains(keywords))
                                            .ToPagedList(page, pageSize);

            return View("Index", productsPagedList);
        }
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Cate)
                .FirstOrDefaultAsync(m => m.ProId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
    }   
}
