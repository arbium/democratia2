using Federation.Core;

namespace System.Web.Mvc
{
    public static class UserContext
    {
        private static DateTime _lastClearTime = DateTime.MinValue;

        public static UserContainer Current
        {
            get
            {
                var userKey = HttpContext.Current.User.Identity.Name;
                var isAuthenticated = HttpContext.Current.Request.IsAuthenticated;

                var user = UserContextService.GetUserContainer(userKey);

                if (user != null)
                {
                    if (isAuthenticated)
                        return user;

                    Abandon(user.Id);
                }
                else if (isAuthenticated)
                {
                    user = UserContextService.CreateNewUserContainer(userKey);

                    if (!UserContextService.RegisterNewUserContainer(userKey, user))
                    {
                        AccountService.SignOut();
                        throw new MvcActionRedirectException("", "home", "index");
                    }

                    return user;
                }

                return null;
            }
        }

        public static void Abandon()
        {
            var key = HttpContext.Current.User.Identity.Name;
            UserContextService.Abandon(key);
        }

        public static void Abandon(Guid userId)
        {
            UserContextService.Abandon(userId);
        }

        public static void GroupMembersAbandon(Guid groupId)
        {
            UserContextService.GroupMembersAbandon(groupId);
        }

        public static void ClearUnused()
        {
            if (_lastClearTime < DateTime.Now.AddMinutes(-15))
            {
                _lastClearTime = DateTime.Now;
                UserContextService.ClearUnused();
            }
        }
    }
}