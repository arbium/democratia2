using System;
using System.Linq;

namespace Federation.Core
{
    public static class SecurityService
    {
        private static ISecurityService _securityService = GetCurrentService();

        private static ISecurityService GetCurrentService()
        {
            var services = ServiceLocator.GetAllInstances<ISecurityService>();

            if (services == null || !services.Any())
                return null;

            ISecurityService currentService = services.FirstOrDefault();
            foreach (var service in services)
            {
                if(service.Version > currentService.Version)
                    currentService = service;
            }
            return currentService;
        }

        public static Guid CalcPasswordHash(string password, Guid salt)
        {
            return _securityService.CalcPasswordHash(password, salt);
        }

        public static void GeneratePasswordHash(string password, out Guid hash, out Guid salt)
        {
            _securityService.GeneratePasswordHash(password, out hash, out salt);
        }

        public static User EmailUserValidation(string email, string password)
        {
            return _securityService.EmailUserValidation(email, password);
        }

        public static User LoginUserValidation(string login, string password)
        {
            return _securityService.LoginUserValidation(login, password);
        }
    }
}
