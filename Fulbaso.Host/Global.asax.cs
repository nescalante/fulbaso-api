using Castle.Facilities.WcfIntegration;
using Castle.Windsor;
using Fulbaso.Service.Contract;
using System;
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
            RouteTable.Routes.Add(new ServiceRoute("util", new DefaultServiceHostFactory(), typeof(ICommonService)));
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
            Container.Install(
                new ServiceInstaller(),
                new AuthenticationInstaller(),
                new ContextInstaller()
            );
        }
    }
}