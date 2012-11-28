using System.Web;
using Federation.Core.Helpers;

namespace Federation.Web.Services
{
    public static class AdminService
    {
        public static bool UserIsAdmin(HttpContextBase httpContext)
        {
            var adminCookie = CookieService.Get(httpContext, "admin");
            if (adminCookie == null)
                return false;

            return AdminKeys.ValidateAdminKey(adminCookie.Value);
        }
    }
}