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

namespace fastfood.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly FoodshopContext _context;
        private readonly IWebHostEnvironment _environment;
        public INotyfService _notifyService { get; }
        public OrdersController(FoodshopContext context, INotyfService notifyService, IWebHostEnvironment environment)
        {
            _context = context;
            _notifyService = notifyService;
            _environment = environment;
        }

        // GET: Admin/Orders
    public IActionResult Index(string name, string diachi, DateTime beginDate = default, DateTime endDate = default, int page1 = 1, int page2 = 1, int page3 = 1, int page4 = 1, int pageSize = 5, int pageId = 1)
    {
            if (beginDate == default)
            {
                beginDate = DateTime.MinValue.Date;
            }

            if (endDate == default)
            {
                endDate = DateTime.MaxValue.Date;
            }

            // Tổng
            var query = _context.Orders.Include(o=>o.Cus).OrderBy(o=>o.OrderDate).ToList();
            //var pagedList = query.ToPagedList(page, pageSize);

            // Tìm kiếm
            if (!string.IsNullOrEmpty(name))
                query = query.Where(o => o.Cus.CusName.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(diachi))
                query = query.Where(h => h.Address.Contains(diachi, StringComparison.OrdinalIgnoreCase)).ToList();

            if (beginDate < endDate)
                query = query.Where(h => h.OrderDate > beginDate && h.OrderDate < endDate).ToList();

            var totalItemCount = query.Count();
            ViewBag.TotalItemCount = totalItemCount;
            //ViewBag.PagedList = pagedList;


            // Chờ xác nhận
            var query1 = query.Where(o => o.Status == "Chờ xác nhận").OrderBy(o=>o.OrderDate);
            var pagedList1 = query1.ToPagedList(page1, pageSize);
            var totalItemCount1 = query1.Count();
            ViewBag.PageStartItem1 = (page1 - 1) * pageSize + 1;
            ViewBag.PageEndItem1 = Math.Min(page1 * pageSize, totalItemCount1);
            ViewBag.TotalItemCount1 = totalItemCount1;
            ViewBag.PagedList1 = pagedList1;

            // Đang giao hàng
            var query2 = query.Where(o => o.Status == "Đang giao");
            var pagedList2 = query2.ToPagedList(page2, pageSize);
            var totalItemCount2 = query2.Count();
            ViewBag.PageStartItem2 = (page2 - 1) * pageSize + 1;
            ViewBag.PageEndItem2 = Math.Min(page2 * pageSize, totalItemCount2);
            ViewBag.TotalItemCount2 = totalItemCount2;
            ViewBag.PagedList2 = pagedList2;

            // Đã giao hàng
            var query3 = query.Where(o => o.Status == "Đã giao");
            var pagedList3 = query3.ToPagedList(page3, pageSize);
            var totalItemCount3 = query3.Count();
            ViewBag.PageStartItem3 = (page3 - 1) * pageSize + 1;
            ViewBag.PageEndItem3 = Math.Min(page3 * pageSize, totalItemCount3);
            ViewBag.TotalItemCount3 = totalItemCount3;
            ViewBag.PagedList3 = pagedList3;

            // Đã hủy
            var query4 = query.Where(o => o.Status == "Hủy");
            var pagedList4 = query4.ToPagedList(page4, pageSize);
            var totalItemCount4 = query4.Count();
            ViewBag.PageStartItem4 = (page4 - 1) * pageSize + 1;
            ViewBag.PageEndItem4 = Math.Min(page4 * pageSize, totalItemCount4);
            ViewBag.TotalItemCount4 = totalItemCount4;
            ViewBag.PagedList4 = pagedList4;

            ViewBag.email = name;
            ViewBag.diachi = diachi;
            ViewBag.beginDate = beginDate;
            ViewBag.endDate = endDate;
            ViewBag.Page1 = page1;
            ViewBag.Page2 = page2;
            ViewBag.Page3 = page3;
            ViewBag.Page4 = page4;
            ViewBag.PageId = pageId;

            return View();
      
    }

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Cus)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Admin/Orders/Create
        [HttpPost]
        public IActionResult XacNhanDonHang(int orderId)
        {
            var donHang = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);

            if (donHang != null)
            {
                // Cập nhật trạng thái đơn hàng thành "Đang giao"
                donHang.Status = "Đang giao";
                _context.SaveChanges(); // Lưu các thay đổi vào cơ sở dữ liệu

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        [HttpPost]
        public IActionResult DaGiao(int orderId)
        {
            var donHang = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);

            if (donHang != null)
            {
                // Cập nhật trạng thái đơn hàng thành "Đang giao"
                donHang.Status = "Đã giao";
                _context.SaveChanges(); // Lưu các thay đổi vào cơ sở dữ liệu

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        [HttpPost]
        public IActionResult Huy(int orderId)
        {
            var donHang = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);

            if (donHang != null)
            {
                // Cập nhật trạng thái đơn hàng thành "Đang giao"
                donHang.Status = "Hủy";
                _context.SaveChanges(); // Lưu các thay đổi vào cơ sở dữ liệu

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var order = _context.Orders.Find(id);

            if (order == null)
            {
                // Trả về JSON với thông báo lỗi nếu không tìm thấy đơn hàng
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
            }

            if (order.Status != "Hủy")
            {
                // Trả về JSON với thông báo lỗi nếu trạng thái không phải là "Hủy"
                return Json(new { success = false, message = "Không thể xóa do trạng thái không phải là 'Hủy'." });
            }

            bool orderDetailsInUse = _context.OrderDetails.Any(p => p.OrderId == id);

            if (orderDetailsInUse)
            {
                // Nếu có chi tiết đơn hàng đang tồn tại, trả về JSON với thông báo lỗi
                return Json(new { success = false, message = "Không thể xóa do có chi tiết thuộc đơn hàng đang tồn tại." });
            }
            else
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();

                // Trả về JSON với thông báo thành công
                return Json(new { success = true, message = "Xóa đơn hàng thành công." });
            }
        }


        // GET: Admin/Orders/Delete/5

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
