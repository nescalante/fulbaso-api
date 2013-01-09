using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Fulbaso.Contract;

namespace Fulbaso.Common.Security
{
    public static class UserExtension
    {
        public static bool IsInRole(this User user, string role)
        {
            var roleService = ContainerUtil.GetApplicationContainer().Resolve<IRoleLogic>();

            return roleService.IsUserInRole(user.Id, role);
        }
    }
}
