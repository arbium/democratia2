using System;
using System.Web;
using System.Web.Routing;

namespace Federation.Web
{
    public class IsGuidConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object value;

            if (!values.TryGetValue(parameterName, out value))
                return false;

            Guid temp;

            return Guid.TryParse(value.ToString(), out temp);
        }
    }
}