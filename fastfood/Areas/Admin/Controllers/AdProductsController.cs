using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fastfood.Models;
using fastfood.Models.ViewModel;
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
        public async Task<IActionResult> Index(string searchTen, string lsp,  double minban, double maxban,  int page = 1, int pageSize = 5)
        {
            ViewBag.ListLoaiSp = await _context.Categories.ToListAsync();
          

            // Get data
            var query = from sp in _context.Products         
                        join ls in _context.Categories on sp.CateId equals ls.CateId
                        select new ProductViewModel()
                        {
                            ProId = sp.ProId,
                            ProName= sp.ProName,
                            ShortDesc = sp.ShortDesc,                          
                            Price = sp.Price,                          
                            Thumb = sp.Thumb,                           
                            CateId = ls.CateId,
                            CateName = ls.CateName,
                            Discount = sp.Discount,
                            Active = sp.Active,
                            Hot  = sp.Hot,
                            Cate = sp.Cate,
                        };

            // Search
            if (!string.IsNullOrEmpty(searchTen))
                query = query.Where(s => s.ProName.Contains(searchTen, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(lsp))
                query = query.Where(s => s.CateName == lsp);

            
            if (maxban != 0)
            {
                query = query.Where(item => item.Price < maxban && item.Price> minban);
            }

            

            var totalItemCount = query.Count();
            var model = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var pagedList = new StaticPagedList<ProductViewModel>(model, page, pageSize, totalItemCount);
            ViewBag.PageStartItem = (page - 1) * pageSize + 1;
            ViewBag.PageEndItem = Math.Min(page * pageSize, totalItemCount);
            ViewBag.TotalItemCount = totalItemCount;
            ViewBag.Page = page;
            ViewBag.searchTen = searchTen;
            ViewBag.lsp = lsp;
           
            ViewBag.minban = minban;
            ViewBag.maxban = maxban;
            
            return View(pagedList);
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
