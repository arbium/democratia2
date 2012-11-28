using System.Web;
using System.Web.Routing;

namespace Federation.Web
{
    public class IsNullConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object value;

            if (!values.TryGetValue(parameterName, out value))
                return true;

            return value == null;
        }
    }
}