using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fulbaso.Contract;

namespace Fulbaso.Common.Security
{
    public class PlaceAuthorizeAttribute : AuthorizeAttribute
    {
        private List<Place> GetPlaces(HttpContextBase httpContext)
        {
            var places = httpContext.Session["Places"];
            if (places == null)
            {
                places = ContainerUtil.GetApplicationContainer().Resolve<IPlaceService>().GetByUser(UserAuthentication.UserId);
            }

            return places as List<Place>;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (UserAuthentication.IsAdmin()) return true;

            var place = httpContext.Request.RequestContext.RouteData.Values["place"];

            if (place != null && httpContext.User.Identity.IsAuthenticated && base.AuthorizeCore(httpContext))
            {
                return ValidatePlace(httpContext, place.ToString());
            }
            else
            {
                return false;
            }
        }

        private bool ValidatePlace(HttpContextBase httpContext, string id)
        {
            var place = ContainerUtil.GetApplicationContainer().Resolve<IPlaceService>().ValidatePage(id);
            var places = this.GetPlaces(httpContext);

            return places != null && places.Any(up => up.Page == place);
        }
    }
}
