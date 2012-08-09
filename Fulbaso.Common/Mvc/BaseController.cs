using System.Web.Mvc;
using Fulbaso.Common;

namespace Fulbaso.UI
{
    public class BaseController : Controller
    {
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (FacebookLogin.User != null)
            {
                filterContext.HttpContext.User = new FacebookPrincipal(new FacebookIdentity(FacebookLogin.User.Name, FacebookLogin.Id, FacebookLogin.Token));
            }

            base.OnAuthorization(filterContext);
        }
    }
}