using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fastfood.Areas.Admin.Controllers
{
    [Authorize(Roles = "nhân viên,quản lý")]
    [Area("Admin")]

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
