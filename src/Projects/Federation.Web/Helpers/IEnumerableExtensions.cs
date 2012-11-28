using System;
using System.Collections.Generic;

namespace Federation.Web
{
    public static class IEnumerableExtensions
    {
        public static PaginationList<T> ToPaginationList<T>(this IEnumerable<T> enumerableInstance, int pageSize, bool isLoadPreviosly = false)
        {
            return new PaginationList<T>(enumerableInstance, pageSize, isLoadPreviosly);
        }

        public static PaginationList<T, TSource> ToPaginationList<T, TSource>(this IEnumerable<TSource> enumerableInstance, int pageSize, Func<TSource, T> transformFunction, bool isLoadPreviosly = false)
        {
            return new PaginationList<T, TSource>(enumerableInstance, pageSize, transformFunction, isLoadPreviosly);
        }
    }
}