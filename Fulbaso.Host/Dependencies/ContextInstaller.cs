using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Fulbaso.EntityFramework;
using System.Configuration;

namespace Fulbaso.Host
{
    public class ContextInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var connectionString = ConfigurationManager.AppSettings["SQLSERVER_CONNECTION_STRING"] ??
                ConfigurationManager.ConnectionStrings["ObjectContextEntities"].ConnectionString;

            container.Kernel.Register(
                Component
                    .For<ObjectContextEntities>()
                    .UsingFactoryMethod(() => new ObjectContextEntities(connectionString))
                    .LifestylePerThread()
                );
        }
    }
}