using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Fulbaso.Contract;

namespace Fulbaso.Common
{
    public class FacebookPrincipal : IPrincipal
    {
        public FacebookPrincipal(FacebookIdentity identity)
        {
            if (identity == null) throw new ArgumentNullException("identity");

            this.Identity = identity;
        }

        public IIdentity Identity
        {
            get;
            private set;
        }

        public bool IsInRole(string role)
        {
            // cast identity
            var identity = this.Identity as FacebookIdentity;

            // not authenticated
            if (identity == null)
            {
                return false;
            }

            // in site role
            if (identity.Roles.Contains(role))
            {
                return true;
            }

            // is in place role
            var place = HttpContext.Current.GetRouteData("place");

            if (place != null)
            {
                int id;
                var container = ContainerUtil.GetApplicationContainer();
                var page = container.Resolve<IPlaceLogic>().ValidatePage(place.ToString(), out id);

                return identity.PlaceRoles.Where(pr => pr.Item1 == id && pr.Item2 == role).Any();
            }

            // user is not in role
            return false;
        }

        public bool IsInRole(string role, int placeId)
        {
            // cast identity
            var identity = this.Identity as FacebookIdentity;

            // not authenticated
            if (identity == null)
            {
                return false;
            }

            // in site role
            if (identity.Roles.Contains(role))
            {
                return true;
            }

            return identity.PlaceRoles.Where(pr => pr.Item1 == placeId && pr.Item2 == role).Any();
        }
    }
}