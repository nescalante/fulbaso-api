using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Fulbaso.Authentication.Logic;
using Fulbaso.EntityFramework.Logic;
using Fulbaso.Facebook.Logic;
using Fulbaso.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fulbaso.Host
{
    public class ServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.Register(
                AllTypes
                    .FromAssemblyContaining<PlaceService>()
                    .Where(t => t.Name.EndsWith("Service"))
                    .WithService.Select(ByConvention)
                    .LifestylePerThread()
                );

            container.Kernel.Register(
                AllTypes
                    .FromAssemblyContaining<PlaceLogic>()
                    .Where(t => t.Name.EndsWith("Logic"))
                    .WithService.Select(ByConvention)
                    .LifestylePerThread()
                );

            container.Kernel.Register(
                AllTypes
                    .FromAssemblyContaining<AlbumLogic>()
                    .Where(t => t.Name.EndsWith("Logic"))
                    .WithService.Select(ByConvention)
                    .LifestylePerThread()
                );

            container.Kernel.Register(
                AllTypes
                    .FromAssemblyContaining<AuthenticationLogic>()
                    .Where(t => t.Name.EndsWith("Logic"))
                    .WithService.Select(ByConvention)
                    .LifestylePerThread()
            );

            container.Kernel.Register(
                AllTypes
                    .FromAssemblyContaining<FacebookLogic>()
                    .Where(t => t.Name.StartsWith("Facebook"))
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

                if (type.Name.EndsWith(name))
                {
                    return new[] { interfaceType };
                }
            }

            return Enumerable.Empty<Type>();
        }
    }
}