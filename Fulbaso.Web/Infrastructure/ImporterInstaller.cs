using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Fulbaso.Importer;

namespace Fulbaso.Web
{
    public class ImporterInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
             Classes.FromAssemblyContaining<IImporter>()
              .BasedOn<IImporter>()
              .WithServiceFirstInterface()
              .LifestylePerThread()
            );

            container.Register(
             Classes.FromThisAssembly()
              .BasedOn<IConsoleOutput>()
              .WithServiceFirstInterface()
              .LifestylePerThread()
            );
        }
    }
}