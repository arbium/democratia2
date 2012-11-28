using System.Linq;

namespace Federation.Core
{
    public interface IDataBox<T> : IQueryable<T> where T : class
    {
        void AddObject(T entity);
        void Attach(T entity);
        void DeleteObject(T entity);
        void Detach(T entity);
        T CreateObject();

    }    
}
