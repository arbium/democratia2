using Federation.Core.Services.MonitoringService;
namespace Federation.Core
{
    public static class AuthenticationService// вынести в аккаунт сервис ... 
    {
        public static IAuthenticationService Current { get; set; }

        public static void SignIn(string userName, bool createPersistentCookie)
        {
            Current.SignIn(userName, createPersistentCookie);
        }

        public static void SignOut()
        {
            Current.SignOut();
        }
    }
}
