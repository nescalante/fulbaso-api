using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Fulbaso.Contract;
using System.Web;
using System.Configuration;

namespace Fulbaso.Common.Security
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return UserAuthentication.IsAdmin();
        }
    }
}
