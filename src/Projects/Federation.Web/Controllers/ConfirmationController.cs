using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Federation.Core;
using Federation.Web.ViewModels;

namespace Federation.Web.Controllers
{
    public class ConfirmationController : MainController
    {
        public ActionResult Action(Guid id)
        {
            ConfirmationData data = ConfirmationService.GetData(id);

            if (data.IsUsed)
                return Redirect(data.ReturnUrl);

            string returnUrl = Request.UrlReferrer.PathAndQuery;

            ConfirmationService.SetReturnUrl(id, returnUrl);

            ConfirmationActionViewModel model = new ConfirmationActionViewModel
            {
                Id = id,
                Message = data.Message,
                NoUrl = returnUrl,
                YesUrl = data.ActionUrl
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Action(Guid id, object[] args)
        {
            ConfirmationData data = ConfirmationService.GetData(id);
            ConfirmationService.SetPostData(id, Request.Form);

            ConfirmationActionViewModel model = new ConfirmationActionViewModel
            {
                Id = id,
                Message = data.Message,
                NoUrl = Request.UrlReferrer.PathAndQuery,
                YesUrl = data.ActionUrl,
                HttpPost = true
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Yes(ConfirmationActionViewModel model)
        {
            if (!model.HttpPost)
                return Redirect(model.YesUrl);

            ConfirmationData data = ConfirmationService.GetData(model.Id);

            RouteData route = RoutesHelper.GetRouteDataByUrl("/" + model.YesUrl);            
            
            //var controllerDescriptor = new ReflectedControllerDescriptor(GetType());
            string controllerName = (String)route.Values["controller"];
            string actionName = (String)route.Values["action"];
            //string values = RouteData.GetRequiredString("id");

            //IControllerActivator
            DefaultControllerFactory d = new DefaultControllerFactory();
            
            IController controller = d.CreateController(HttpContext.Request.RequestContext, controllerName);

            ControllerDescriptor controllerDescriptor = new ReflectedControllerDescriptor(controller.GetType());
            //d.ReleaseController(controller);

            ActionDescriptor actionDescriptor = controllerDescriptor.FindAction(ControllerContext, actionName);

            RequestContext requestContext = new RequestContext(new RoutesHelper.RewritedHttpContextBase("/" + model.YesUrl), route);

            requestContext.HttpContext.Request.Form.Add((NameValueCollection)data.PostData);
            
            ControllerContext ctx = new ControllerContext(requestContext, (ControllerBase)controller);
            IDictionary<string, object> parameters2 = GetParameterValues(ctx, actionDescriptor);
            IDictionary<string, object> parameters = new Dictionary<string,object>();

            ControllerContext.HttpContext.Response.Clear();
            NameValueCollection nameValueCollection = data.PostData as NameValueCollection;
            //nameValueCollection.
            actionDescriptor.Execute(ControllerContext, (IDictionary<string, object>)data.PostData);

            //var viewResult = new ViewResult
            //{
            //    ViewName = "",
            //    MasterName = "",
            //    ViewData = new ViewDataDictionary(data.PostData),
            //    TempData = null
            //};         

            //return viewResult;
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult No(ConfirmationActionViewModel model)
        {
            ConfirmationService.DeleteRecord(model.Id);

            return Redirect(model.NoUrl);
        }
    }
}
