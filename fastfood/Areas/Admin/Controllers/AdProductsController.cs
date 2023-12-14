using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fastfood.Models;
using X.PagedList.Mvc.Core;
using System.Drawing.Printing;
using X.PagedList;
using fastfood.Helpper;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace fastfood.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdProductsController : Controller
    {
        private readonly FoodshopContext _context;
        private readonly IWebHostEnvironment _environment;
        public INotyfService _notifyService { get; }
        public AdProductsController(FoodshopContext context, INotyfService notifyService, IWebHostEnvironment environment)
        {
            _context = context;
            _notifyService = notifyService;
            _environment = environment;
        }

        // GET: Admin/AdProducts
        [HttpGet]
        public async Task<IActionResult> Index(string searchInput, double? to, double? from, int page = 1)
        {
            page = page < 1 ? 1 : page;
            int pageSize = 4;

            var productsQuery = _context.Products.Include(p=>p.Cate).AsQueryable();

                if (!string.IsNullOrEmpty(searchInput))
                {
                    productsQuery = productsQuery.Where(x => x.ProName.Contains(searchInput));
                }

                if (to != null && from != null)
                {
                    productsQuery = productsQuery.Where(x => x.Price >= to && x.Price <= from);
                }
                else if (to != null)
                {
                    productsQuery = productsQuery.Where(x => x.Price >= to);
                }
                else if (from != null)
                {
                    productsQuery = productsQuery.Where(x => x.Price <= from);
                }
            

            var productsPagedList = await productsQuery.ToPagedListAsync(page, pageSize);

            return View(productsPagedList);
        }
       

        public class ProductViewModel
        {
            public IPagedList<Product> Products { get; set; }
            public int CateId { get; set; }
            // Other properties...
        }

        // GET: Admin/AdProducts/Details/5
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

        // GET: Admin/AdProducts/Create
        public IActionResult Create()
        {
            ViewData["CateId"] = new SelectList(_context.Categories, "CateId", "CateName");
            return View();
        }

        // POST: Admin/AdProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProId,ProName,ShortDesc,Price,Discount,CateId,CreateDate,Thumb,Title,Hot,Active")] Product product, IFormFile fThumb)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                // Thêm logic xử lý lỗi nếu cần
            }

            // Kiểm tra điều kiện không đáp ứng cho việc cần cập nhập ảnh đại diện hoặc nhập đúng giá trị
            if (ModelState.IsValid && (fThumb == null || string.IsNullOrEmpty(product.ProName)))
            {
                // Xử lý khi ModelState không hợp lệ
                _notifyService.Error("Cần cập nhập ảnh đại diện hoặc nhập đúng giá trị");
                ViewData["CateId"] = new SelectList(_context.Categories, "CateId", "CateId", product.CateId);
                return View(product);
            }

            if (ModelState.IsValid)
            {
                product.ProName = Utilities.ToTitleCase(product.ProName);

                // Thiết lập giá trị của trường "hot" và "active" từ checkbox
               

                // Xử lý upload hình ảnh
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(product.ProName) + extension;
                    product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                }

                // Xử lý khi không có ảnh đại diện
                if (string.IsNullOrEmpty(product.Thumb))
                {
                    product.Thumb = "default.jpg";
                }

                // Thêm sản phẩm vào cơ sở dữ liệu
                _context.Add(product);
                await _context.SaveChangesAsync();

                _notifyService.Success("Thêm mới thành công");
                return RedirectToAction(nameof(Index));
            }

            // Xử lý khi ModelState không hợp lệ
            _notifyService.Error("Cần cập nhập ảnh đại diện hoặc nhập đúng giá trị");
            ViewData["CateId"] = new SelectList(_context.Categories, "CateId", "CateName", product.CateId);
            return View(product);
        }
        // GET: Admin/AdProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CateId"] = new SelectList(_context.Categories, "CateId", "CateName", product.CateId);
            return View(product);
        }

        // POST: Admin/AdProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProId,ProName,ShortDesc,Price,Discount,CateId,CreateDate,Hot,Thumb,Active,Title")] Product product, IFormFile? fThumb)
        {
            if (id != product.ProId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                // Thêm logic xử lý lỗi nếu cần
            }

            // Kiểm tra điều kiện không đáp ứng cho việc cần cập nhập ảnh đại diện hoặc nhập đúng giá trị
            if (ModelState.IsValid && (fThumb == null || string.IsNullOrEmpty(product.ProName)))
            {
                // Xử lý khi ModelState không hợp lệ
                _notifyService.Error("Cần cập nhập ảnh đại diện hoặc nhập đúng giá trị");
                ViewData["CateId"] = new SelectList(_context.Categories, "CateId", "CateId", product.CateId);
                return View(product);
            }

            if (ModelState.IsValid)
            {
                product.ProName = Utilities.ToTitleCase(product.ProName);

                // Xử lý upload hình ảnh
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(product.ProName) + extension;
                    product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                }

                // Thêm sản phẩm vào cơ sở dữ liệu
                _context.Update(product);
                await _context.SaveChangesAsync();

                _notifyService.Success("Cập nhật thành công");
                return RedirectToAction(nameof(Index));
            }

            _notifyService.Error("Cần cập nhập ảnh đại diện hoặc nhập đúng giá trị");
            ViewData["CateId"] = new SelectList(_context.Categories, "CateId", "CateName", product.CateId);
            return View(product);
        }

        // GET: Admin/AdProducts/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = _context.Products.Find(id);
            if (item != null)
            {
                try
                {
                    _context.Products.Remove(item);
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

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.ProId == id)).GetValueOrDefault();
        }
    }
}
