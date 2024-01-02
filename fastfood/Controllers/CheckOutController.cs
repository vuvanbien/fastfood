using fastfood.Infrastructure;
using fastfood.Models;
using fastfood.Models.CheckOut;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fastfood.Controllers
{
    [Authorize(Roles = "khách hàng")]
    public class CheckOutController : Controller
    {
        public Cart? Cart { get; set; }
         public List<CartLine> Lines { get; set; }
        private readonly FoodshopContext _context;

        public CheckOutController(FoodshopContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            var username = HttpContext.Session.GetString("UserName");
            int cusId = HttpContext.Session.GetInt32("CusId") ?? 0;
            var customer = _context.Customers.FirstOrDefault(u => u.CusId == cusId);
            
            var checkView = new CheckView
            {
                Customer = customer,
                Cart = Cart,
            };
            return View("Index", checkView);
        }
        [HttpPost]
        public IActionResult Index(string diachi, string note)
        {
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            var username = HttpContext.Session.GetString("UserName");
            int cusId = HttpContext.Session.GetInt32("CusId") ?? 0;
            var customer = _context.Customers.FirstOrDefault(u => u.CusId == cusId);
            var checkView = new CheckView
            {
                Customer = customer,
                Cart = Cart,
            };

            if (username == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            else
            {
                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    Note = note,
                    Address = diachi,
                    Status = "Chờ xác nhận",
                    TotalMoney = (double)Cart.ComputeTotalValue(),
                    CusId = cusId
                };
                _context.Add(order);
                _context.SaveChanges();
                List<CartLine> cartLines = Cart.Lines;

        /*        // Lấy các dòng từ session
                List<CartLine> lines = HttpContext.Session.GetJson<List<CartLine>>("Cart") ?? new List<CartLine>();*/
                foreach (var cartLine in cartLines)
                {
                    var orderdetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProId = cartLine.Product.ProId,
                        ProName = cartLine.Product.ProName,
                        Price = cartLine.Product.Price,
                        Discount = cartLine.Product.Discount,
                        Quantity = cartLine.Quantity,
                        TotalMoney = cartLine.Quantity * cartLine.Product.Price

                    };
                    _context.Add(orderdetail);
                    _context.SaveChanges();
                    // Thêm mỗi chi tiết đơn hàng vào ngữ cảnh

                }

                // Lưu các thay đổi vào ngữ cảnh
                

                // Xóa session
                HttpContext.Session.Remove("cart");

                TempData["success"] = "Thanh toán thành công, vui lòng chờ duyệt đơn hàng";
                /*return View("Index", checkView);*/
                return RedirectToAction("Index", "Cart", checkView);
            }
        }
    }
}
