using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Federation.Web
{
    public static class HtmlHelperExtension
    {
        public static IHtmlString Paginator<T>(this HtmlHelper helper, PaginationList<T> paginationList, string urlPrefix = null)
        {
            var currentUrl = HttpContext.Current.Request.Url;

            if (urlPrefix == null)
                urlPrefix = currentUrl.Scheme + "://" + currentUrl.Host + currentUrl.AbsolutePath;

            var totalRecords = paginationList.TotalCount;
            var currentPageSize = paginationList.Count;
            var currentPage = paginationList.CurrentPageNumber;

            var pageCount = 0;
            if (currentPageSize != 0)
                pageCount = Math.Max((int)Math.Ceiling((double)totalRecords / currentPageSize), 1);

            var resultString = new StringBuilder();
            var httpItemsString = string.Empty;

            var queryString = helper.ViewContext.HttpContext.Request.QueryString;
            for (var i = 0; i < queryString.Count; i++)
            {
                if (queryString.Keys[i] != "page")
                    httpItemsString += "&" + queryString.Keys[i] + "=" + queryString[i];
            }

            if (currentPage > 0)
                resultString.AppendLine(String.Format("<a href=\"{0}?page={1}{2}\">Предыдущая</a>", urlPrefix, currentPage, httpItemsString));

            if (pageCount > 1)
            {
                for (var i = 0; i < pageCount; i++)
                {
                    if (i == 0)
                    {
                        if (i != currentPage)
                            resultString.AppendLine(String.Format("<a href=\"{0}?page={1}{2}\">{1}</a>", urlPrefix, i + 1, httpItemsString));
                        else
                            resultString.AppendLine(String.Format("<a href=\"{0}?page={1}{2}\"><b>{1}</b></a>", urlPrefix, i + 1, httpItemsString));
                    }
                    else
                    {
                        if (i != currentPage)
                            resultString.AppendLine(String.Format("<a href=\"{0}?page={1}{2}\">{1}</a>", urlPrefix, i + 1, httpItemsString));
                        else
                            resultString.AppendLine(String.Format("<a href=\"{0}?page={1}{2}\"><b>{1}</b></a>", urlPrefix, i + 1, httpItemsString));
                    }
                }
            }

            if (currentPage + 1 < pageCount)
                resultString.AppendLine(String.Format("<a href=\"{0}?page={1}{2}\">Следующая</a>", urlPrefix, currentPage + 2, httpItemsString));

            return helper.Raw(resultString.ToString());
        }

        public static IHtmlString PageLoadButton<T>(this HtmlHelper helper, PaginationList<T> paginationList, string urlPrefix = null)
        {
            var currentUrl = HttpContext.Current.Request.Url;

            if (string.IsNullOrEmpty(urlPrefix))
                urlPrefix = currentUrl.Scheme + "://" + currentUrl.Host + currentUrl.AbsolutePath;

            var totalRecords = paginationList.TotalCount;
            var currentPageSize = paginationList.Count;
            var currentPage = paginationList.CurrentPageNumber;
            var firstNewItemNumber = (currentPage + 1) * currentPageSize + 1;

            var pageCount = 0;
            if (currentPageSize != 0)
                pageCount = Math.Max((int)Math.Ceiling((double)totalRecords / currentPageSize), 1);

            var resultString = string.Empty;
            var httpItemsString = string.Empty;

            var queryString = helper.ViewContext.HttpContext.Request.QueryString;
            for (var i = 0; i < queryString.Count; i++)
            {
                if (queryString.Keys[i] != "page" && queryString.Keys[i] != "ajax")
                    httpItemsString += "&" + queryString.Keys[i] + "=" + queryString[i];
            }
            httpItemsString += "&ajax=true";

            if (currentPage < pageCount && pageCount != 1 && currentPage != pageCount - 1)
                resultString = String.Format("<a class=\"paginator action-button\" href=\"#\" url=\"{0}?page={1}{2}#item{3}\" title=\"Подгрузка контента не работает с выключенным javascript\" style=\"opacity: 0.5\">Еще</a>", 
                    urlPrefix, currentPage + 2, httpItemsString, firstNewItemNumber);

            return helper.Raw(resultString);
        }
    }
}