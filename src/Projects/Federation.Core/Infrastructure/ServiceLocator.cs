using System;
using System.Collections.Generic;

namespace Federation.Core
{
    public static class ServiceLocator // TODO: refactoring with shared & non-shared
    {
        private static Func<IServiceLocator> _serviceLocatorProvider = DefaultServiceLocatorProvider;
        private static IServiceLocator DefaultServiceLocatorProvider()
        {
            throw new InvalidOperationException("Service provider is not defined");
        }

        public static void SetProvider(Func<IServiceLocator> serviceLocatorProvider)
        {
            if (serviceLocatorProvider == null) // DoNothing
                throw new ArgumentNullException("serviceLocatorProvider");
            _serviceLocatorProvider = serviceLocatorProvider;
        }

        public static IServiceLocator Current
        {
            get { return _serviceLocatorProvider(); }
        }

        public static IEnumerable<T> GetAllInstances<T>()
        {
            return Current.GetAllInstances<T>();
        }

        public static T GetInstance<T>()
        {
            return Current.GetInstance<T>();
        }

        public static T GetInstance<T>(string key)
        {
            return Current.GetInstance<T>(key);
        }

        public static T GetNewInstance<T>() where T : class
        {
            return Current.GetNewInstance<T>();
        }

        public static T GetNewInstance<T>(string key) where T : class
        {
            return Current.GetNewInstance<T>(key);
        }
    }
}
