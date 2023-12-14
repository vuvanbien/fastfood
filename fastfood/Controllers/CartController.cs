using fastfood.Infrastructure;
using fastfood.Models;
using fastfood.Models.CheckOut;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace fastfood.Controllers
{
    public class CartController : Controller
    {
        public Cart? Cart { get; set; }
       
        private readonly FoodshopContext _context;
        public CartController(FoodshopContext context)
        {
            _context = context;
        }
        // GET: CartController
        public IActionResult Index()
        {
            
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            return View("Index", Cart);
        }
        public IActionResult Add(int ProId)
        {
            Product? product = _context.Products.FirstOrDefault(p => p.ProId == ProId);
            if (product != null)
            {
                Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
                Cart.AddItem(product, 1);
                HttpContext.Session.SetJson("cart", Cart);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
            }

            return RedirectToAction("Index", "Products");
        }
        public IActionResult Remove(int ProId)
        {
            Product? product = _context.Products.FirstOrDefault(p => p.ProId == ProId);
            if (product != null)
            {
                Cart = HttpContext.Session.GetJson<Cart>("cart");
                Cart.RemoveLine(product);
                HttpContext.Session.SetJson("cart", Cart);
            }
            return View("Index", Cart);
        }
        // GET: CartController/Details/5
        

    }
}
