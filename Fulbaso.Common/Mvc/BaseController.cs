using System.Web.Mvc;

namespace Fulbaso.Common
{
    public class BaseController : Controller
    {
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Authentication.User != null)
            {
                filterContext.HttpContext.User = new FacebookPrincipal(new FacebookIdentity(Authentication.User));
            }

            base.OnAuthorization(filterContext);
        }
    }
}