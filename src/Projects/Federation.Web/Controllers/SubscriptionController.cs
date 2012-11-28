using System;
using System.Web.Mvc;
using Federation.Core;
using AuthenticationException = System.Security.Authentication.AuthenticationException;

namespace Federation.Web.Controllers
{
    public class SubscriptionController : MainController
    {
        public ActionResult SubscribeToGroup(string id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            SubscriptionService.SubscribeToGroup(id, UserContext.Current.Id);
            UserContext.Abandon();

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.PathAndQuery);

            return RedirectToAction("editsubscription", "user");
        }

        public ActionResult SubscribeToUser(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            SubscriptionService.SubscribeToUser(id, UserContext.Current.Id);
            UserContext.Abandon();

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.PathAndQuery);

            return RedirectToAction("editsubscription", "user");
        }

        public ActionResult UnsubscribeFromGroup(string id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            SubscriptionService.UnsubscribeFromGroup(id, UserContext.Current.Id);
            UserContext.Abandon();

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.PathAndQuery);

            return RedirectToAction("editsubscription", "user");
        }

        public ActionResult UnsubscribeFromUser(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            SubscriptionService.UnsubscribeFromUser(id, UserContext.Current.Id);
            UserContext.Abandon();

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.PathAndQuery);

            return RedirectToAction("editsubscription", "user");
        }
    }
}