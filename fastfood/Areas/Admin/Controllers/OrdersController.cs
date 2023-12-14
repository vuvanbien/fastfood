using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fastfood.Models;
using PagedList;
using X.PagedList;

namespace fastfood.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly FoodshopContext _context;

        public OrdersController(FoodshopContext context)
        {
            _context = context;
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index(int page = 1)
        {
            page = page < 1 ? 1 : page;
            int pageSize = 9;

            var orderPagedList =await _context.Orders.Include(o=>o.Cus).ToPagedListAsync(page, pageSize);

            return View(orderPagedList);
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



        // GET: Admin/Orders/Edit/5


        // GET: Admin/Orders/Delete/5

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
