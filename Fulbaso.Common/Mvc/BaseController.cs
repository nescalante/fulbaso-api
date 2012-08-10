using System.Web.Mvc;

namespace Fulbaso.Common
{
    public class BaseController : Controller
    {
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (FacebookLogin.User != null)
            {
                filterContext.HttpContext.User = new FacebookPrincipal(new FacebookIdentity(FacebookLogin.User));
            }

            base.OnAuthorization(filterContext);
        }
    }
}