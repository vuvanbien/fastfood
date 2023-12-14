using fastfood.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using fastfood.Components;
using fastfood.Models;
using fastfood.Models.ViewModel;
using X.PagedList;


namespace ShopPet.Controllers
{
    public class CategoryController : Controller
    {
        
        private readonly FoodshopContext _context;
        public CategoryController(FoodshopContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string searchTen, List<string> lsp, int page = 1, int pageSize = 6)
        {
           
            ViewBag.ListLoaiSp = await _context.Categories.ToListAsync();
            

            var query = from sp in _context.Products                       
                        join ls in _context.Categories on sp.CateId equals ls.CateId
                        select new ProductViewModel()
                        {
                            ProId = sp.ProId,
                            ProName = sp.ProName,
                            ShortDesc = sp.ShortDesc,                          
                            Price = sp.Price,
                            Thumb = sp.Thumb,
                            Active = sp.Active,
                            Hot = sp.Hot,
                            Discount=sp.Discount,
                            CateId = ls.CateId,
                            CateName = ls.CateName,
                           
                        };

            if (!string.IsNullOrEmpty(searchTen))
            {
                query = query.Where(s => s.ProName.Contains(searchTen));
            }

            if (lsp != null && lsp.Any())
            {
                query = query.Where(s => lsp.Contains(s.CateName));
            }
            var totalItemCount = await query.CountAsync();
            var model = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var pagedList = new StaticPagedList<ProductViewModel>(model, page, pageSize, totalItemCount);

            ViewBag.PageStartItem = (page - 1) * pageSize + 1;
            ViewBag.PageEndItem = Math.Min(page * pageSize, totalItemCount);
            ViewBag.Page = page;
            ViewBag.TotalItemCount = totalItemCount;
            ViewBag.searchTen = searchTen;
            ViewBag.lsp = lsp;
           

            return View(pagedList);
        }
    }
}
