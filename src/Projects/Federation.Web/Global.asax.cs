using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Federation.Core;
using Federation.Web.Services;

namespace Federation.Web
{
    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Group",
                "group/{id}/{action}",
                new { controller = "group", action = "index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "CurrUser",
                "user",
                new { controller = "user", action = "index", id = UrlParameter.Optional },
                new { id = new NotConstraint(new IsNullConstraint()) }
            );

            routes.MapRoute(
                "User",
                "user/id{id}",
                new { controller = "user", action = "index", id = UrlParameter.Optional },
                new { id = new IsGuidConstraint() }
            );            

            //routes.MapRoute(
            //    "User",
            //    "user/{id}/{action}",
            //    new { controller = "user", action = "index", id = UrlParameter.Optional }
            //);

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "home", action = "index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            InitializeApplication();
        }

        protected void Application_End()
        {
            //File.Save
        }

        private void InitializeApplication()
        {
            new Bootstrapper().Run();
            AuthenticationService.Current = new AuthenticationServiceImpl();

            ConstHelper.AppPath = HttpContext.Current.Server.MapPath("/");
            ConstHelper.AppUrl = "http://democratia2.ru"; //"http://tismo.su";//
            ConstHelper.HomeUrl = "http://democratia2.ru/"; //"http://tismo.su";//
            ConstHelper.LoginUrl = ConstHelper.HomeUrl + "account/signin";// "http://tismo.su/accont/signin";//"http://federation.ru/accont/signin";

            ConstHelper.UrlAliases.Add("federation.ru");
            ConstHelper.UrlAliases.Add("democratia2.ru");
            ConstHelper.UrlAliases.Add("www.democratia2.ru");
            ConstHelper.UrlAliases.Add("democratia2.org");
            ConstHelper.UrlAliases.Add("www.democratia2.org");
            ConstHelper.UrlAliases.Add("democracy2.ru");
            ConstHelper.UrlAliases.Add("www.democracy2.ru");

            DataService.PerThread.BeginWork();

            SmsService.UseHttps = true;
            SmsService.UsePost = true;
            SmsService.Authorize("", "");

            ScheduleService.Initialize();
            ScheduleService.Start();
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            Session["init"] = 1;
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.CurrentExecutionFilePathExtension))
            {
                DataService.PerThread.BeginWork();
            }
        }

        protected void Application_EndRequest(Object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.CurrentExecutionFilePathExtension))
            {
                DataService.PerThread.EndWork();
            }
        }
    }
}