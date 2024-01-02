using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fastfood.Models;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;

namespace fastfood.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "quản lý")]
    public class AdAccountsController : Controller
    {
        private readonly FoodshopContext _context;

        public AdAccountsController(FoodshopContext context)
        {
            _context = context;
        }

        // GET: Admin/AdAccounts
        [HttpGet]
        public async Task<IActionResult> Index(string? search,int page = 1 )
        {
            page = page < 1 ? 1 : page;
            int pagesize = 5;
            var acc = from c in _context.Accounts.Include(a => a.Role).Include(a => a.Cus)

            select c;
            if (!string.IsNullOrEmpty(search))
            {
                acc = acc.Where(x => x.Cus.CusName.Contains(search));
            }
            var accPagedList = await acc.ToPagedListAsync(page, pagesize);
            return View(accPagedList);
            
        }

        // GET: Admin/AdAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.Cus)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Admin/AdAccounts/Create
        public IActionResult Create()
        {
            // Check if _context.Roles is not null before creating SelectList
            if (_context.Roles != null)
            {
                ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            }
            else
            {
                // Handle the case where _context.Roles is null (provide a default behavior or log an error)
                // For example, you can set ViewData["RoleId"] to an empty SelectList or handle it based on your requirements.
                ViewData["RoleId"] = new SelectList(new List<Role>(), "RoleId", "RoleName");
            }

            return View();
        }
        // POST: Admin/AdAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,Password,CreateDate,RoleId,Active,CusId")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Use RoleName for display and RoleId for the actual value
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }
        // GET: Admin/AdAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            return View(account);
        }

        // POST: Admin/AdAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserName,Password,CreateDate,RoleId,Active,CusId")] Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            return View(account);
        }

        // GET: Admin/AdAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/AdAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Accounts == null)
            {
                return Problem("Entity set 'FoodshopContext.Accounts'  is null.");
            }
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
          return (_context.Accounts?.Any(e => e.AccountId == id)).GetValueOrDefault();
        }
    }
}
