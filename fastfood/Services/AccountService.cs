using fastfood.Models;

namespace fastfood.Services
{
    public class AccountService : IAccountService
    {
        private readonly FoodshopContext _context;

        public AccountService(FoodshopContext context)
        {
            _context = context;
        }

        public List<string> GetRolesByUsername(string username)
        {
            var user = _context.Accounts.SingleOrDefault(a => a.UserName == username);

            if (user != null && user.RoleId != null)
            {
                var roles = _context.Roles
                    .Where(r => r.RoleId == user.RoleId)
                    .Select(r => r.RoleName)
                    .ToList();

                return roles;
            }

            return new List<string>();
        }
        // Các phương thức khác
    }
}
