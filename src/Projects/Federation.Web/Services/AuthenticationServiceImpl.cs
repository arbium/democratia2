using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Federation.Core;
using Federation.Core.Services.MonitoringService;

namespace Federation.Web.Services
{
    public class AuthenticationServiceImpl : IAuthenticationService
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentException("Value cannot be null or empty.", "userName");
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
            UserContext.ClearUnused();//Очищаем повисший контекст пользователей
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
            HttpCookieCollection cookies = HttpContext.Current.Request.Cookies;
            int sumCookie = cookies.Count;
            for (int i = 0; i < sumCookie; i++)
            {
                HttpCookie cookie = cookies[i];
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }

            UserContext.Abandon(); //Удалить текущий контекст пользователя
            HttpContext.Current.Session.Abandon();
        }
    }
}