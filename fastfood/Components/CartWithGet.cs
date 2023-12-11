using fastfood.Infrastructure;
using fastfood.Models;

using Microsoft.AspNetCore.Mvc;

namespace fastfood.Components
{
    public class CartWithGet : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View(HttpContext.Session.GetJson<Cart>("cart"));
        }
    }
}
