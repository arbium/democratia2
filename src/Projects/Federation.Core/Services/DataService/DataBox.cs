using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;

namespace Federation.Core
{
    public class DataBox<T> : IDataBox<T> where T : class
    {
        private ObjectSet<T> _objSet;

        public DataBox(ObjectSet<T> objSet)
        {
            _objSet = objSet;
        }

        public void AddObject(T entity)
        {
            _objSet.AddObject(entity);
        }

        public void Attach(T entity)
        {
            _objSet.Attach(entity);
        }

        public T CreateObject()
        {
            return _objSet.CreateObject();
        }

        public void DeleteObject(T entity)
        {
            _objSet.DeleteObject(entity);
        }

        public void Detach(T entity)
        {
            _objSet.Detach(entity);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_objSet).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_objSet).GetEnumerator();
        }

        public Type ElementType
        {
            get
            {
                return ((IQueryable<T>)_objSet).ElementType;
            }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get
            {
                return ((IQueryable<T>)_objSet).Expression;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return ((IQueryable<T>)_objSet).Provider;
            }
        }
    }
}
