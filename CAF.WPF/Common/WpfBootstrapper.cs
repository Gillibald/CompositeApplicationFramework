using CompositeApplicationFramework.Base;
using CompositeApplicationFramework.Interfaces;
using Unity;
using Unity.Lifetime;

namespace CompositeApplicationFramework.Common
{
    public class WpfBootstrapper : BootstrapperBase
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<IPlatform, WpfPlatform>(new ContainerControlledLifetimeManager());

            Container.RegisterType<IResourceLoader, WpfResourceLoader>(new ContainerControlledLifetimeManager());
        }
    }
}
