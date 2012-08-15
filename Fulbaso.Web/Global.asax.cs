using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using Fulbaso.Common;
using log4net;

namespace Fulbaso.UI
{
    public class MvcApplication : HttpApplication, IContainerAccessor
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(HttpApplication));
        private static IWindsorContainer _container;

        public IWindsorContainer Container
        {
            get { return _container; }
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.Map();
        }

        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;
            ModelMetadataProviders.Current = new DataAnnotationsModelMetadataProvider();
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            InitializeDependencies();
        }

        private void InitializeDependencies()
        {
            _container = new WindsorContainer();
            _container.Install(
                new ServiceInstaller(),
                new AuthenticationInstaller(),
                new ControllerInstaller()
            );

            WindsorControllerFactory controllerFactory = new WindsorControllerFactory(_container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }
    }
}

