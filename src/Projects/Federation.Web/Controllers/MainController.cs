using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Federation.Core;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Federation.Web
{
    public class MainController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            object errorObj = Request[ConstHelper.ErrorCode];
            if (errorObj != null)
            {
                var errorKey = errorObj.ToString();
                var errorText = ErrorService.Get(errorKey);
                ViewBag.ExceptionText = errorText;

            }

            if (UserContext.Current != null && !UserContext.Current.IsPhoneVerified)
            {
                if (!Request.Path.Contains("/home/") && !Request.Path.Contains("/account/") && !Request.Path.Contains("/about") && !Request.Path.Contains("/help") && !Request.Path.Contains("/feedback"))
                {
                    var result = RedirectToAction("activation", "account", null);
                    filterContext.Result = result;
                }
            }

            //if (UserContext.Current != null && UserContext.Current.IsOutdated)
            //{
            //    if (!Request.Path.Contains("/home/") && !Request.Path.Contains("/account/") && !Request.Path.Contains("/about") && !Request.Path.Contains("/help") && !Request.Path.Contains("/feedback"))
            //    {
            //        var result = RedirectToAction("activation", "account", null);
            //        filterContext.Result = result;
            //    }
            //}

            base.OnActionExecuting(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            //TODO: подумать о том чтобы делать параметризированный ендВорк без савечангес

            var exception = filterContext.Exception;

            Logger.Write("Message: " + exception.Message + "\r\n Data: \r\n" + exception.Data + "\r\n Trace:\r\n" + exception.StackTrace, "Exceptions", 0, 32667, TraceEventType.Error);//TODO: подкрутить вывод в лог

            #region Системные ошибки
            if (exception is SystemException)
            {
                var result = new ViewResult//TODO: Глобальная ошбика, например отключение бд, привелет к показу! Нужно редиректить на специальный лайаут, без пользовательских данных, который точно не ебнеться, и адльше смотреть, если нет коннекта к БД писать прямо об этом
                {
                    ViewName = "Error",
                    MasterName = "",
                    ViewData = null,
                    TempData = filterContext.Controller.TempData
                };

                result.ViewBag.Exception = exception;
                result.ViewBag.ExceptionText =
                    "Возникла ошибка, обратитесь к команде разработчиков через <a href='" + Url.Action("index", "feedback") + "'>форму обратной связи</a> или по <a href='mailto:support@democratia2.ru'>email</a>";

                filterContext.Result = result;
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = 500;
            }
            #endregion

            #region Ошибки приложения
            if (exception is ApplicationException)
            {
                #region Ошибки редиректа

                if (filterContext.Exception is RedirectException)
                {
                    RedirectException redirectException = (RedirectException)filterContext.Exception;

                    string url = "";

                    if (!string.IsNullOrWhiteSpace(redirectException.RedirectUrl))
                        url = redirectException.RedirectUrl;
                    else
                        url = ConstHelper.HomeUrl;


                    List<string> urlParameters = new List<string>();

                    if (!String.IsNullOrEmpty(redirectException.Message))
                        urlParameters.Add(ConstHelper.ErrorCode + "=" + ErrorService.Add(redirectException.Message));

                    if (filterContext.Exception is AuthenticationException)
                        urlParameters.Add("returnUrl" + "=" + HttpUtility.UrlEncode(HttpContext.Request.Url.ToString()));//TODO: вынести returl или внести ErrorCode

                    StringBuilder finallyUrl = new StringBuilder(url);
                    finallyUrl.Append("?");
                    for (int i = 0; i < urlParameters.Count; i++)
                    {
                        finallyUrl.Append(urlParameters[i]);

                        if (i != urlParameters.Count - 1)
                            finallyUrl.Append("&");
                    }

                    filterContext.Result = new RedirectResult(finallyUrl.ToString());
                    filterContext.ExceptionHandled = true;
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusCode = 500;
                    return;
                }

                if (filterContext.Exception is MvcActionRedirectException)
                {
                    MvcActionRedirectException mvcRredirectException = (MvcActionRedirectException)filterContext.Exception;
                    var result = RedirectToAction(mvcRredirectException.ActionName, mvcRredirectException.ControllerName, mvcRredirectException.RouteValues);

                    if (!String.IsNullOrEmpty(mvcRredirectException.Message))
                        result.RouteValues.Add(ConstHelper.ErrorCode, ErrorService.Add(mvcRredirectException.Message));

                    filterContext.Result = result;
                    filterContext.ExceptionHandled = true;
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusCode = 500;
                    return;
                }

                #endregion

                #region Ошибки ведущие на остование на этой же странице

                #region Ошибки бизнес логики
                //чтобы заюзать внутренний кеш, можно поробовать обкаст к коетроллер акстион енвокер
                if (filterContext.Exception is BusinessLogicException)
                {
                    BusinessLogicException businessLogicException = (BusinessLogicException)filterContext.Exception;

                    var controllerDescriptor = new ReflectedControllerDescriptor(GetType());
                    string actionName = RouteData.GetRequiredString("action");
                    ActionDescriptor actionDescriptor = controllerDescriptor.FindAction(ControllerContext, actionName);
                    IDictionary<string, object> parameters = GetParameterValues(ControllerContext, actionDescriptor);

                    object model = null;
                    if (parameters.Keys.Contains("model"))
                        model = parameters["model"];

                    var viewResult = new ViewResult
                    {
                        ViewName = "",
                        MasterName = "",
                        ViewData = new ViewDataDictionary(model),
                        TempData = filterContext.Controller.TempData
                    };

                    viewResult.ViewBag.Exception = businessLogicException;
                    viewResult.ViewBag.ExceptionText = String.Format(businessLogicException.Message);

                    ControllerContext context = ControllerContext;
                    try
                    {
                        viewResult.ExecuteResult(context);
                    }
                    catch//TODO: URLREFERER case
                    {
                        //if (HttpContext.Request.UrlReferrer != null)
                        //{
                        //    if(Core.UrlHelper.IsInnerUrl(HttpContext.Request.UrlReferrer.ToString()))
                        //    {
                        //    }

                        //}
                        //else
                        //{
                        var result = new ViewResult//TODO: Глобальная ошбика, например отключение бд, привелет к показу! Нужно редиректить на специальный лайаут, без пользовательских данных, который точно не ебнеться, и адльше смотреть, если нет коннекта к БД писать прямо об этом
                        {
                            ViewName = "Error",
                            MasterName = "",
                            ViewData = null,
                            TempData = filterContext.Controller.TempData
                        };

                        result.ViewBag.Exception = businessLogicException;
                        result.ViewBag.ExceptionText = String.Format(businessLogicException.Message);

                        filterContext.Result = result;
                        filterContext.ExceptionHandled = true;
                        filterContext.HttpContext.Response.Clear();
                        filterContext.HttpContext.Response.StatusCode = 500;
                        return;
                        //}
                    }

                    filterContext.Result = viewResult;
                    filterContext.ExceptionHandled = true;
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusCode = 500;
                    return;
                }
                #endregion

                #region Ошибки валидации
                //чтобы заюзать внутренний кеш, можно поробовать обкаст к коетроллер акстион енвокер
                if (filterContext.Exception is ValidationException)
                {
                    ValidationException validationException = (ValidationException)filterContext.Exception;

                    var controllerDescriptor = new ReflectedControllerDescriptor(GetType());
                    string actionName = RouteData.GetRequiredString("action");
                    ActionDescriptor actionDescriptor = controllerDescriptor.FindAction(ControllerContext, actionName);
                    IDictionary<string, object> parameters = GetParameterValues(ControllerContext, actionDescriptor);

                    object model = null;
                    if (parameters.Keys.Contains("model"))
                        model = parameters["model"];

                    var viewResult = new ViewResult
                    {
                        ViewName = "",
                        MasterName = "",
                        ViewData = new ViewDataDictionary(model),
                        TempData = filterContext.Controller.TempData
                    };

                    viewResult.ViewBag.ValidationException = validationException;
                    viewResult.ViewBag.ValidationExceptionText = String.Format(validationException.Message);
                    //viewResult.ExecuteResult();
                    filterContext.Result = viewResult;
                    filterContext.ExceptionHandled = true;
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusCode = 500;
                    return;
                }

                #endregion

                #endregion
            }
            #endregion

            base.OnException(filterContext);
        }

        protected virtual IDictionary<string, object> GetParameterValues(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            Dictionary<string, object> parametersDict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            ParameterDescriptor[] parameterDescriptors = actionDescriptor.GetParameters();

            foreach (ParameterDescriptor parameterDescriptor in parameterDescriptors)
            {
                parametersDict[parameterDescriptor.ParameterName] = GetParameterValue(controllerContext, parameterDescriptor);
            }
            return parametersDict;
        }

        protected virtual object GetParameterValue(ControllerContext controllerContext, ParameterDescriptor parameterDescriptor)
        {
            // collect all of the necessary binding properties
            Type parameterType = parameterDescriptor.ParameterType;
            IModelBinder binder = GetModelBinder(parameterDescriptor);
            IValueProvider valueProvider = controllerContext.Controller.ValueProvider;
            string parameterName = parameterDescriptor.BindingInfo.Prefix ?? parameterDescriptor.ParameterName;
            Predicate<string> propertyFilter = GetPropertyFilter(parameterDescriptor);

            ModelBindingContext bindingContext = new ModelBindingContext()
            {
                FallbackToEmptyPrefix = (parameterDescriptor.BindingInfo.Prefix == null), // only fall back if prefix not specified
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, parameterType),
                ModelName = parameterName,
                ModelState = controllerContext.Controller.ViewData.ModelState,
                PropertyFilter = propertyFilter,
                ValueProvider = valueProvider
            };

            object result = binder.BindModel(controllerContext, bindingContext);
            return result ?? parameterDescriptor.DefaultValue;
        }

        private IModelBinder GetModelBinder(ParameterDescriptor parameterDescriptor)
        {
            return parameterDescriptor.BindingInfo.Binder ?? Binders.GetBinder(parameterDescriptor.ParameterType);
        }

        private static Predicate<string> GetPropertyFilter(ParameterDescriptor parameterDescriptor)
        {
            ParameterBindingInfo bindingInfo = parameterDescriptor.BindingInfo;
            return propertyName => IsPropertyAllowed(propertyName, bindingInfo.Include.ToArray(), bindingInfo.Exclude.ToArray());
        }

        private static bool IsPropertyAllowed(string propertyName, string[] includeProperties, string[] excludeProperties)
        {
            bool includeProperty = (includeProperties == null) || (includeProperties.Length == 0) || includeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
            bool excludeProperty = (excludeProperties != null) && excludeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
            return includeProperty && !excludeProperty;
        }
    }
}
