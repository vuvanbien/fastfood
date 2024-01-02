using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fastfood.Models;
using X.PagedList;
using fastfood.Helpper;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace fastfood.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PagesController : Controller
    {
        private readonly FoodshopContext _context;
        private readonly IWebHostEnvironment _environment;
        public INotyfService _notifyService { get; }
        public PagesController(FoodshopContext context, INotyfService notifyService, IWebHostEnvironment environment)
        {
            _context = context;
            _notifyService = notifyService;
            _environment = environment;
        }

        // GET: Admin/Pages
        public async Task<IActionResult> Index(string? searchInput, int page = 1)
        {
            page = page < 1 ? 1 : page;
            int pagesize = 8;
            var pages = from c in _context.Pages
                       select c;
            if (!string.IsNullOrEmpty(searchInput))
            {
                pages = pages.Where(x => x.PageName.Contains(searchInput));
            }
            var pagePagedList = await pages.ToPagedListAsync(page, pagesize);
            return View(pagePagedList);
        }

        // GET: Admin/Pages/Details/5
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

        // GET: Admin/Pages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Pages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PageId,PageName,Contents,Thumb,Title,CreateDate")] Page page, IFormFile fThumb)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                // Thêm logic xử lý lỗi nếu cần
            }

            // Kiểm tra điều kiện không đáp ứng cho việc cần cập nhập ảnh đại diện hoặc nhập đúng giá trị
            if (ModelState.IsValid && (fThumb == null || string.IsNullOrEmpty(page.PageName)))
            {
                // Xử lý khi ModelState không hợp lệ
                _notifyService.Error("Cần cập nhập ảnh đại diện hoặc nhập đúng giá trị");
               
                return View(page);
            }

            if (ModelState.IsValid)
            {
                page.PageName = Utilities.ToTitleCase(page.PageName);

                // Thiết lập giá trị của trường "hot" và "active" từ checkbox
                page.Contents = Utilities.SanitizeHtml(page.Contents);


                // Xử lý upload hình ảnh
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(page.PageName) + extension;
                    page.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                }

                // Xử lý khi không có ảnh đại diện
                if (string.IsNullOrEmpty(page.Thumb))
                {
                    page.Thumb = "default.jpg";
                }

                // Thêm sản phẩm vào cơ sở dữ liệu
                _context.Add(page);
                await _context.SaveChangesAsync();

                _notifyService.Success("Thêm mới thành công");
                return RedirectToAction(nameof(Index));
            }

            // Xử lý khi ModelState không hợp lệ
            _notifyService.Error("Cần cập nhập ảnh đại diện hoặc nhập đúng giá trị");
            return View(page);
        }

        // GET: Admin/Pages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                return NotFound();
            }

            var page = await _context.Pages.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }
            return View(page);
        }

        // POST: Admin/Pages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PageId,PageName,Contents,Thumb,Title,CreateDate")] Page page, IFormFile? fThumb)
        {
            if (id != page.PageId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                // Thêm logic xử lý lỗi nếu cần
            }

            // Kiểm tra điều kiện không đáp ứng cho việc cần cập nhập ảnh đại diện hoặc nhập đúng giá trị
            if (ModelState.IsValid && (fThumb == null || string.IsNullOrEmpty(page.PageName)))
            {
                // Xử lý khi ModelState không hợp lệ
                _notifyService.Error("Cần cập nhập ảnh đại diện hoặc nhập đúng giá trị");
               
                return View(page);
            }

            if (ModelState.IsValid)
            {
                page.PageName = Utilities.ToTitleCase(page.PageName);

                // Xử lý upload hình ảnh
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(page.PageName) + extension;
                    page.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                }

                // Thêm sản phẩm vào cơ sở dữ liệu
                _context.Update(page);
                await _context.SaveChangesAsync();

                _notifyService.Success("Cập nhật thành công");
                return RedirectToAction(nameof(Index));
            }

            _notifyService.Error("Cần cập nhập ảnh đại diện hoặc nhập đúng giá trị");
            
            return View(page);
        }

        // GET: Admin/Pages/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = _context.Pages.Find(id);
            if (item != null)
            {
                try
                {
                    _context.Pages.Remove(item);
                    _context.SaveChanges();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi nếu có
                    Console.WriteLine(ex.Message);
                    return Json(new { success = false, error = ex.Message });
                }
            }
            return Json(new { success = false });
        }

        private bool PageExists(int id)
        {
          return (_context.Pages?.Any(e => e.PageId == id)).GetValueOrDefault();
        }
    }
}
