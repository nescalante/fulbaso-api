using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Fulbaso.Common.Security;

namespace Fulbaso.Host
{
    public class AuthenticationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.Register(
                Classes
                    .FromAssemblyContaining<UserAuthentication>()
                    .Where(t => t.Name.EndsWith("Authentication"))
                    .LifestylePerThread()
                );
        }
    }
}