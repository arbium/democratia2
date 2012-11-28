using System.Web.Mvc.Html;
using Federation.Core;

namespace System.Web.Mvc
{
    public static class MvcHelperExtension
    {
        public static string ConfirmAction(this UrlHelper helper, string message, string actionUrl)
        {
            var confirmId = ConfirmationService.GetId(
                new ConfirmationData
                {
                    Message = message,
                    ActionUrl = actionUrl
                });

            return helper.Action("action", "confirmation", new { id = confirmId });
        }

        public static MvcHtmlString ConfirmActionLink(this HtmlHelper helper, string linkText, string message, string actionUrl, object htmlAttributes = null)
        {
            var confirmId = ConfirmationService.GetId(
                new ConfirmationData
                {
                    Message = message,
                    ActionUrl = actionUrl
                });

            return helper.ActionLink(linkText, "action", "confirmation", new { id = confirmId }, htmlAttributes); 
        }
    }
}