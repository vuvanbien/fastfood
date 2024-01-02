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
        public IActionResult Index(int page1 = 1, int page2 = 1, int pageSize1 = 3,int pageSize2 = 6 ,int pageId = 1)
        {
            var query = _context.Products.Include(p => p.Cate).ToList();
            // Giảm giá
            var query1 = query.Where(o => o.Discount != null && o.Discount != 0);
            var pagedList1 = query1.ToPagedList(page1, pageSize1);
            var totalItemCount1 = query1.Count();
            ViewBag.PageStartItem1 = (page1 - 1) * pageSize1 + 1;
            ViewBag.PageEndItem1 = Math.Min(page1 * pageSize1, totalItemCount1);
            ViewBag.TotalItemCount1 = totalItemCount1;
            ViewBag.PagedList1 = pagedList1;

            // Không giảm
            var query2 = query.Where(o => o.Discount == null || o.Discount == 0);
            var pagedList2 = query2.ToPagedList(page2, pageSize2);
            var totalItemCount2 = query2.Count();
            ViewBag.PageStartItem2 = (page2 - 1) * pageSize2 + 1;
            ViewBag.PageEndItem2 = Math.Min(page2 * pageSize2, totalItemCount2);
            ViewBag.TotalItemCount2 = totalItemCount2;
            ViewBag.PagedList2 = pagedList2;

            ViewBag.Page1 = page1;
            ViewBag.Page2 = page2;
            ViewBag.PageId = pageId;

            return View();
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
