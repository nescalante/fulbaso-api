using System.Web;
using System.Web.Routing;

namespace Fulbaso.Common
{
    public static class ContextUtil
    {
        public static RouteData GetRouteData(this HttpContext httpContext)
        {
            if (httpContext != null && httpContext.Request != null)
            {
                return httpContext.Request.RequestContext.RouteData;
            }
            else
            {
                return null;
            }
        }

        public static object GetRouteData(this HttpContext httpContext, string value)
        {
            var routeData = httpContext.GetRouteData();

            if (routeData != null)
            {
                return routeData.Values[value];
            }
            else
            {
                return null;
            }
        }
    }
}
