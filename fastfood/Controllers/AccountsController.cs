using fastfood.Models;
using Microsoft.AspNetCore.Mvc;
using fastfood.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Foodweb.Controllers
{
    public class AccountsController : Controller
    {
        private readonly FoodshopContext _context;


        public AccountsController(FoodshopContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public IActionResult Login(Account user)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                var u = _context.Accounts
                    .Where(x => x.UserName.Equals(user.UserName) && x.Password.Equals(user.Password))
                    .FirstOrDefault();

                if (u != null)
                {
                    HttpContext.Session.SetString("UserName", u.UserName.ToString());
                   
                    HttpContext.Session.SetInt32("AccountId", u.AccountId);
                    HttpContext.Session.SetInt32("RoleId", u.RoleId ?? 0); // Lưu RoleId vào session
                    HttpContext.Session.SetInt32("CusId", u.CusId ?? 0);
                    ViewData["UserName"] = u.UserName.ToString();

                    if (u.RoleId == 3)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                }
            }

            return View();
        }
        [HttpGet]
        public IActionResult Logout()
        {
            // Xóa dữ liệu từ Session
            HttpContext.Session.Clear();

            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("Login");
        }


    }
}
