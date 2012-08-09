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

            if (context == null)
            {
                throw new InvalidOperationException(Resources.Error.InvalidContext);
            }

            IContainerAccessor accessor = context.ApplicationInstance as IContainerAccessor;

            if (accessor == null)
            {
                throw new InvalidOperationException(Resources.Error.NoContainerAccessor);
            }

            if (accessor.Container == null)
            {
                throw new InvalidOperationException(Resources.Error.NoContainerInitialized);
            }

            return accessor.Container;
        }
    }
}