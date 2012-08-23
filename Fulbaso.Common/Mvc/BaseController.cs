using System.Web.Mvc;
using Fulbaso.Common.Security;

namespace Fulbaso.Common
{
    public class BaseController : Controller
    {
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (UserAuthentication.User != null)
            {
                filterContext.HttpContext.User = new FacebookPrincipal(new FacebookIdentity(UserAuthentication.User));
            }

            base.OnAuthorization(filterContext);
        }
    }
}