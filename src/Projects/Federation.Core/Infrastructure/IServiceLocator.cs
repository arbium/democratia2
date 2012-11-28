using System.Collections.Generic;

namespace Federation.Core
{
    public interface IServiceLocator
    {
        T GetInstance<T>();
        T GetInstance<T>(string key);
        T GetNewInstance<T>() where T : class;
        T GetNewInstance<T>(string key) where T : class;
        IEnumerable<T> GetAllInstances<T>();        
    }
}
