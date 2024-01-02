using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fastfood.Models;
using X.PagedList;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace fastfood.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdCategoriesController : Controller
    {
        private readonly FoodshopContext _context;
        private readonly IWebHostEnvironment _environment;
        public INotyfService _notifyService { get; }
        public AdCategoriesController(FoodshopContext context, INotyfService notifyService, IWebHostEnvironment environment)
        {
            _context = context;
            _notifyService = notifyService;
            _environment = environment;
        }

        // GET: Admin/AdCategories
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            page = page < 1 ? 1 : page;
            int pagesize = 2;
            var cate = from c in _context.Categories
                      select c;
            if (!string.IsNullOrEmpty(search))
            {
                cate = cate.Where(x => x.CateName.Contains(search));
            }
            var productsPagedList = await cate.ToPagedListAsync(page, pagesize);
            return View(productsPagedList);
        }

        // GET: Admin/AdCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CateId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/AdCategories/Create
        [Authorize(Roles ="quản lý")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "quản lý")]
        public async Task<IActionResult> Create([Bind("CateId,CateName,Status,Sort,ParentId,Metakeyword,MetaDesc,CreateBy,CreateDate,UpdateBy,UpdateDate")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/AdCategories/Edit/5
        [Authorize(Roles = "quản lý")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/AdCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "quản lý")]
        public async Task<IActionResult> Edit(int id, [Bind("CateId,CateName,Status,Sort,ParentId,Metakeyword,MetaDesc,CreateBy,CreateDate,UpdateBy,UpdateDate")] Category category)
        {
            if (id != category.CateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CateId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/AdCategories/Delete/5
        [Authorize(Roles = "quản lý")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CateId == id);

            if (category == null)
            {
                return NotFound();
            }

            // Kiểm tra xem có sản phẩm nào sử dụng Category này không
            bool categoryInUse = _context.Products.Any(p => p.CateId == id);

            if (categoryInUse)
            {
                // Nếu có sản phẩm sử dụng Category này, trả về thông báo
                _notifyService.Error("Không thể xóa do có sản phẩm thuộc loại đang tồn tại");
                return RedirectToAction("Index"); // hoặc trả về một trang thông báo lỗi khác
            }

            // Nếu không có sản phẩm sử dụng Category này, hiển thị View bình thường
            return View(category);
        }

        // POST: Admin/AdCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "quản lý")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'FoodContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.CateId == id)).GetValueOrDefault();
        }
    }
}
