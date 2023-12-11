using fastfood.Models;
using fastfood.Models.PageViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace fastfood.Controllers
{
    public class PagesController : Controller
    {
        private readonly FoodshopContext _context;
        public int PageSize = 9;


        public PagesController(FoodshopContext context)
        {
            _context = context;
        }
        // GET: PagesController
        public ActionResult Index(int page =1 )
        {
            return View(
            new PageListViewModel
               {
                   Pages = _context.Pages
                   .Skip((page - 1) * PageSize)
                   .Take(PageSize),
                   PageInfo = new PageInfo
                   {
                       PagePerPage = PageSize,
                       CurrentPage = page,
                       TotalPage = _context.Pages.Count()
                   }
               }
               );
        }

        // GET: PagesController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                return NotFound();
            }

            var page = await _context.Pages
               
                .FirstOrDefaultAsync(m => m.PageId == id);

            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }
        // GET: PagesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PagesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PagesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PagesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PagesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PagesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
