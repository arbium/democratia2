using System.Collections.Specialized;
using System.Web.Routing;

namespace System.Web.Mvc
{
    public static class RoutesHelper
    {
        private const string _theme = "base";

        public static string CurrentThemeFolder
        {
            get { return "/Resources/themes/" + _theme; }
        }

        public static string ScriptFolder
        {
            get { return "/Resources/scripts"; }
        }

        public static RouteData GetRouteDataByUrl(string url)
        {
            return RouteTable.Routes.GetRouteData(new RewritedHttpContextBase(url));
        }

        public class RewritedHttpContextBase : HttpContextBase
        {
            private readonly HttpRequestBase mockHttpRequestBase;

            public RewritedHttpContextBase(string appRelativeUrl)
            {
                mockHttpRequestBase = new MockHttpRequestBase(appRelativeUrl);
            }

            public override HttpRequestBase Request
            {
                get { return mockHttpRequestBase; }
            }

            private class MockHttpRequestBase : HttpRequestBase
            {
                private readonly string appRelativeUrl;
                private NameValueCollection _form = new NameValueCollection();

                public MockHttpRequestBase(string appRelativeUrl)
                {
                    this.appRelativeUrl = appRelativeUrl;
                }

                public override string AppRelativeCurrentExecutionFilePath
                {
                    get { return appRelativeUrl; }
                }

                public override NameValueCollection Form
                {
                    get { return _form; }
                }

                public override string PathInfo
                {
                    get { return ""; }
                }
            }
        }
    }
}