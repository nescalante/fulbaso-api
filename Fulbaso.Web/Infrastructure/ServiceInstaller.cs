using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Fulbaso.Authentication.Logic;
using Fulbaso.EntityFramework.Logic;

namespace Fulbaso.Web
{
    public class ServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
             AllTypes
              .FromAssemblyContaining<PlaceService>()
              .Where(t => t.Name.EndsWith("Service"))
              .WithService.Select(ByConvention)
              .LifestylePerThread()
            );

            container.Register(
             AllTypes
              .FromAssemblyContaining<AuthenticationService>()
              .Where(t => t.Name.EndsWith("Service"))
              .WithService.Select(ByConvention)
              .LifestylePerThread()
            );
        }

        private IEnumerable<Type> ByConvention(Type type, Type[] types)
        {
            Type[] interfaces = type.GetInterfaces();
            foreach (Type interfaceType in interfaces)
            {
                string name = interfaceType.Name;
                if (name.StartsWith("I"))
                {
                    name = name.Remove(0, 1);
                }

                if (type.Name.EndsWith(name) || name.EndsWith("EntityService"))
                {
                    return new[] { interfaceType };
                }
            }

            return Enumerable.Empty<Type>();
        }
    }
}