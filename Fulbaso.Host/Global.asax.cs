using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Fulbaso.Authentication.Logic;
using Fulbaso.Common.Security;
using Fulbaso.EntityFramework.Logic;
using Fulbaso.Facebook.Logic;
using Fulbaso.Service;
using Fulbaso.Service.Contract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;

namespace Fulbaso.Host
{
    public class Global : HttpApplication
    {
        public static WindsorContainer Container { get; set; }

        protected void Application_Start(object sender, EventArgs e)
        {
            BuildContainer();

            RouteTable.Routes.Add(new ServiceRoute("places", new DefaultServiceHostFactory(), typeof(IPlaceService)));
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoStore();
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");

            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.End();
            }
        }

        private void BuildContainer()
        {
            Container = new WindsorContainer();
            Container.Kernel.AddFacility<WcfFacility>();
            Container.Kernel.Register(
                AllTypes
                    .FromAssemblyContaining<PlaceService>()
                    .Where(t => t.Name.EndsWith("Service"))
                    .WithService.Select(ByConvention)
                    .LifestylePerThread()
                );

            Container.Kernel.Register(
                AllTypes
                    .FromAssemblyContaining<PlaceLogic>()
                    .Where(t => t.Name.EndsWith("Logic"))
                    .WithService.Select(ByConvention)
                    .LifestylePerThread()
                );

            Container.Kernel.Register(
                AllTypes
                    .FromAssemblyContaining<AlbumLogic>()
                    .Where(t => t.Name.EndsWith("Logic"))
                    .WithService.Select(ByConvention)
                    .LifestylePerThread()
                );

            Container.Kernel.Register(
                AllTypes
                    .FromAssemblyContaining<AuthenticationLogic>()
                    .Where(t => t.Name.EndsWith("Logic"))
                    .WithService.Select(ByConvention)
                    .LifestylePerThread()
            );

            Container.Kernel.Register(
                AllTypes
                    .FromAssemblyContaining<FacebookLogic>()
                    .Where(t => t.Name.StartsWith("Facebook"))
                    .LifestylePerThread()
                );

            Container.Kernel.Register(
                Classes
                    .FromAssemblyContaining<UserAuthentication>()
                    .Where(t => t.Name.EndsWith("Authentication"))
                    .LifestylePerThread()
                );

            Container.Kernel.Register(
                Component
                    .For<Fulbaso.EntityFramework.ObjectContextEntities>()
                    .UsingFactoryMethod(() => new Fulbaso.EntityFramework.ObjectContextEntities(ConfigurationManager.AppSettings["SQLSERVER_CONNECTION_STRING"])) // ConfigurationManager.ConnectionStrings["ObjectContextEntities"].ConnectionString))
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