using System.Security.Claims;
using System.Security.Principal;

namespace fastfood.Extension
{
    public static class IdentityExtentions
    {
        public static string GetAccountID(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("AccountId");
            return (claim != null) ? claim.Value :string.Empty;
        }
        public static string GetRoleID(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("RoleId");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetUserName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("UserName");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetAvartar(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Avartar");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetSpecificClaim(this ClaimsPrincipal claimsPrincipal, string claimType)
        {
            var claim= claimsPrincipal.Claims.FirstOrDefault(x=>x.Type==claimType);
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}
