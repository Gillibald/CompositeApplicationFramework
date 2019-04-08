using CompositeApplicationFramework.Base;
using CompositeApplicationFramework.Interfaces;

using Unity;
using Unity.Lifetime;

namespace CompositeApplicationFramework.Common
{
    public class AvaloniaBootstrapper : BootstrapperBase
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<IPlatform, AvaloniaPlatform>(new ContainerControlledLifetimeManager());

            Container.RegisterType<IResourceLoader, AvaloniaResourceLoader>(new ContainerControlledLifetimeManager());
        }
    }
}
