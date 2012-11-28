using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;

namespace Federation.Core.Tests
{
    public class FakeDataBox<T> : IDataBox<T> where T : class
    {
        public BlockingCollection<T> InnerList { get; private set; }
        public IQueryable<T> FakeQueryable { get { return InnerList.AsQueryable<T>(); } }

        public FakeDataBox(BlockingCollection<T> fakeList)
        {
            InnerList = fakeList;
        }

        public void AddObject(T entity)
        {
            InnerList.Add(entity);
        }

        public void Attach(T entity)
        {
            throw new NotImplementedException();
        }

        public void DeleteObject(T entity)
        {
            while (InnerList.TryTake(out entity))
            {
                Thread.Sleep(1);
            }
        }

        public void Detach(T entity)
        {
            throw new NotImplementedException();
        }

        public T CreateObject()
        {
            return Activator.CreateInstance<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)InnerList).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)InnerList).GetEnumerator();
        }

        public Type ElementType
        {
            get { return FakeQueryable.ElementType; }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return FakeQueryable.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return FakeQueryable.Provider; }
        }
    }
}
