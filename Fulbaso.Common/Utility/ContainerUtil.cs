using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Windsor;

namespace Fulbaso.Common
{
    public class ContainerUtil
    {
        public static IWindsorContainer GetApplicationContainer()
        {
            HttpContext context = HttpContext.Current;
            IContainerAccessor accessor = context.ApplicationInstance as IContainerAccessor;

            return accessor.Container;
        }
    }
}