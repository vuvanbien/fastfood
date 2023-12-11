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
        public int PageSize = 9;


        public ProductsController(FoodshopContext context)
        {
            _context = context;
        }

        // GET: Products
        public IActionResult Index(int page = 1)
        {

            return View(
                new ProductListViewModel
                {
                    Products = _context.Products
                    .Include(p => p.Cate)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize),
                    PagingInfo = new PagingInfo
                    {
                        ItemPerPage = PageSize,
                        CurrentPage = page,
                        TotalItems = _context.Products.Count()
                    }
                }
                );

        }
        [HttpPost]
        public IActionResult Search(string keywords, int page = 1)
        {
            int totalItems = _context.Products
                                 .Include(p => p.Cate)
                                 .Where(p => p.ProName.Contains(keywords))
                                 .Count();

            return View("Index",
                new ProductListViewModel
                {
                    Products = _context.Products
                                    .Include(p => p.Cate)
                                    .Where(p => p.ProName.Contains(keywords))
                                    .Skip((page - 1) * PageSize)
                                    .Take(PageSize),
                    PagingInfo = new PagingInfo
                    {
                        ItemPerPage = PageSize,
                        CurrentPage = page,
                        TotalItems = totalItems
                    }
                }
            );
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
