using System;
using System.Linq;
using System.Web;

namespace Federation.Web.Services
{
    public static class CookieService
    {
        public static void Create(HttpContextBase httpContext, string name, string value, DateTime? expires)
        {
            HttpCookie cookie = new HttpCookie(name) { Value = value };
            if (expires != null)
                cookie.Expires = expires.Value;
            httpContext.Response.Cookies.Add(cookie);
        }

        public static void Remove(HttpContextBase httpContext, string name)
        {
            if (httpContext.Request.Cookies.AllKeys.Contains(name))
            {
                HttpCookie cookie = httpContext.Request.Cookies[name];
                cookie.Expires = DateTime.Now.AddDays(-1);
                httpContext.Response.Cookies.Add(cookie);
            }
        }

        public static HttpCookie Get(HttpContextBase httpContext, string name)
        {
            return httpContext.Request.Cookies[name];
        }

        public static bool Contains(HttpContextBase httpContext, string name)
        {
            return httpContext.Request.Cookies.AllKeys.Contains(name);
        }
    }
}