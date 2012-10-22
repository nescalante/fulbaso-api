using System.Web.Mvc;
using Fulbaso.Common.Security;

namespace Fulbaso.Common
{
    public class BaseController : Controller
    {
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            var user = ContainerUtil.GetApplicationContainer().Resolve<UserAuthentication>().GetUser();

            if (user != null)
            {
                filterContext.HttpContext.User = new FacebookPrincipal(new FacebookIdentity(user));
            }

            base.OnAuthorization(filterContext);
        }
    }
}