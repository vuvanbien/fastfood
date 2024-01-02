namespace fastfood.Services
{
    public interface IAccountService
    {
        List<string> GetRolesByUsername(string username);
        // Các phương thức khác liên quan đến tài khoản
    }
}
