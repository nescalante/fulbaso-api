using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Fulbaso.Authentication.Logic;
using Fulbaso.Common;
using Fulbaso.Common.Security;
using Fulbaso.EntityFramework.Logic;
using Fulbaso.Facebook.Logic;

namespace Fulbaso.Web
{
    public class ServiceInstaller : IWindsorInstaller
    {
        private IWindsorContainer _container;

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            _container = container;

            container.Register(
             AllTypes
              .FromAssemblyContaining<PlaceLogic>()
              .Where(t => t.Name.EndsWith("Logic"))
              .WithService.Select(ByConvention)
              .LifestylePerThread()
            );

            container.Register(
             AllTypes
              .FromAssemblyContaining<AlbumLogic>()
              .Where(t => t.Name.EndsWith("Logic"))
              .WithService.Select(ByConvention)
              .LifestylePerThread()
            );

            container.Register(
             AllTypes
              .FromAssemblyContaining<AuthenticationLogic>()
              .Where(t => t.Name.EndsWith("Logic"))
              .WithService.Select(ByConvention)
              .LifestylePerThread()
            );

            container.Register(
                Component
                    .For<ExceptionHelper>()
                    .ImplementedBy<ExceptionHelper>()
                    .LifestyleTransient()
            );

            container.Register(
                Component
                    .For<HttpContextBase>()
                    .UsingFactoryMethod(() => this.GetContext())
                    .LifestylePerWebRequest()
            );
        }

        public HttpContextWrapper GetContext()
        {
            var user = _container.Resolve<UserAuthentication>().GetUser();

            if (user != null)
            {
                HttpContext.Current.User = new FacebookPrincipal(new FacebookIdentity(user));
            }

            return new HttpContextWrapper(HttpContext.Current);
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