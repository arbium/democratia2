using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Federation.Web
{
    public class PaginationList<T> : IList<T>
    {
        protected IList<T> InnerList;
        protected int PageSize;

        public int CurrentPageNumber { get; protected set; } // Под вопросом - нужно ли гдето в рантайме менять кол-во итемов, выводимых на страницу. По моим соображением, т.к у нас все упрятано во вью-моделях, логично эти поля заполнять один раз и в конструкторе
        public int TotalCount { get; protected set; }
        public bool IsLoadPreviosly { get; protected set; }

        public int FeedCount
        {
            get
            {
                var count = (CurrentPageNumber + 1) * PageSize;
                return Math.Min(count, TotalCount);
            }
        }

        public PaginationList(IEnumerable<T> enumerableInstance, int pageSize, bool isLoadPreviosly = false, int? currentPageNumber = null) // Не уверен стоит ли делать отдельно такую же штуку для квериабле, есть подозрение что при обкастоке к энумирабле выполнется запрос в БД
        {
            if (enumerableInstance == null)
                return;

            int pageNumber;

            if (currentPageNumber.HasValue)
                pageNumber = currentPageNumber.Value;
            else
                Int32.TryParse(HttpContext.Current.Request["page"], out pageNumber);

            enumerableInstance = enumerableInstance.ToList();

            if (enumerableInstance is IQueryable<T>)
                TotalCount = ((IQueryable<T>)enumerableInstance).Count();
            else
                TotalCount = enumerableInstance.Count();

            CurrentPageNumber = pageNumber;
            PageSize = pageSize;
            IsLoadPreviosly = isLoadPreviosly;            

            if (IsLoadPreviosly)
            {
                var firstItemNumber = (pageNumber + 1) * pageSize;
                InnerList = enumerableInstance.Take(firstItemNumber).ToList();
            }
            else
            {
                var firstItemNumber = pageNumber * pageSize;
                InnerList = enumerableInstance.Skip(firstItemNumber).Take(pageSize).ToList();
                //Возможно стоит использовать копирование эл-в, чтобы еще больше отвязаться от функции ToList, реализация которой лежит на совести реализующего интерфейс IEnumerable
            }
        }

        public PaginationList() : this(new List<T>(), 0 )
        {   
        }

        public int IndexOf(T item)
        {
            return InnerList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            InnerList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            InnerList.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return InnerList[index]; }
            set { InnerList[index] = value; }
        }

        public void Add(T item)
        {
            InnerList.Add(item);
        }

        public void Clear()
        {
            InnerList.Clear();
        }

        public bool Contains(T item)
        {
            return InnerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            InnerList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return PageSize; }
        }

        public bool IsReadOnly
        {
            get { return InnerList.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            return InnerList.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)InnerList).GetEnumerator();
        }
    }

    public class PaginationList<T, TSource> : PaginationList<T>
    {
        public PaginationList()
        {
        }

        public PaginationList(IEnumerable<TSource> enumerableInstance, int pageSize, Func<TSource, T> transformFunction, bool isLoadPreviosly = false) //Не уверен стоит ли делать отдельно такую же штуку для квериабле, есть подозрение что при обкастоке к энумирабле выполнется запрос в БД
        {
            if (enumerableInstance == null)
                return;

            int pageNumber;
            Int32.TryParse(HttpContext.Current.Request["page"], out pageNumber); // potetonal Exception

            if (pageNumber == 0)
                pageNumber = 1;

            pageNumber--;

            CurrentPageNumber = pageNumber;
            PageSize = pageSize;
            IsLoadPreviosly = isLoadPreviosly;

            enumerableInstance = enumerableInstance.ToList();

            if (enumerableInstance is IQueryable<TSource>)
                TotalCount = ((IQueryable<TSource>)enumerableInstance).Count();
            else 
                TotalCount = enumerableInstance.Count();                       

            if (IsLoadPreviosly)
            {
                var firstItemNumber = (pageNumber + 1) * pageSize;
                InnerList = enumerableInstance.Take(firstItemNumber).Select(transformFunction).ToList();
            }
            else
            {
                var firstItemNumber = pageNumber * pageSize;
                InnerList = enumerableInstance.Skip(firstItemNumber).Take(pageSize).Select(transformFunction).ToList();
                //Возможно стоит использовать копирование эл-в, чтобы еще больше отвязаться от функции ToList, реализация которой лежит на совести реализующего интерфейс IEnumerable
            }
        }
    }
}