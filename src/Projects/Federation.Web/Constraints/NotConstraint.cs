using System.Web;
using System.Web.Routing;

namespace Federation.Web
{
    public class NotConstraint : IRouteConstraint
    {
        private readonly IRouteConstraint _constraint;

        public NotConstraint(IRouteConstraint constraint)
        {
            _constraint = constraint;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return !_constraint.Match(httpContext, route, parameterName, values, routeDirection);
        }
    }
}