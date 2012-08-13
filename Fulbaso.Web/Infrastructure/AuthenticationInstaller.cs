using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Fulbaso.Facebook.Logic;
using Fulbaso.Common;

namespace Fulbaso.UI
{
    public class AuthenticationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
             AllTypes
              .FromAssemblyContaining<FacebookService>()
              .Where(t => t.Name.StartsWith("Facebook"))
              .LifestylePerThread()
            );

            container.Register(
             Classes
              .FromAssemblyContaining<UserAuthentication>()
              .Where(t => t.Name.EndsWith("Authentication"))
              .LifestylePerThread()
            );
        }
    }
}