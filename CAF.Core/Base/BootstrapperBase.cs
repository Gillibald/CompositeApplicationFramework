#region Usings

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CompositeApplicationFramework.Events;
using CompositeApplicationFramework.Interfaces;
using CompositeApplicationFramework.Loggers;
using CompositeApplicationFramework.Module;
using CompositeApplicationFramework.Navigation;
using CompositeApplicationFramework.ViewModel;
using Unity;
using Unity.Lifetime;

#endregion

namespace CompositeApplicationFramework.Base
{
    using System.Threading.Tasks;

    /// <summary>
    ///     Bootstrapper base
    /// </summary>
    public abstract class BootstrapperBase : IBootstrapper
    {
        /// <summary>
        ///     Main shell
        /// </summary>
        public IShell Shell { get; protected set; }

        /// <summary>
        ///     Logger
        /// </summary>
        public ILoggerFacade Logger { get; protected set; }

        /// <summary>
        ///     IOC Container
        /// </summary>
        public IUnityContainer Container { get; protected set; }

        /// <summary>
        ///     ModuleCatalog
        /// </summary>
        public IModuleCatalog ModuleCatalog { get; protected set; }

        /// <summary>
        ///     Platform
        /// </summary>
        public IPlatform Platform { get; protected set; }

        /// <summary>
        ///     NavigationService
        /// </summary>
        public INavigationService NavigationService { get; protected set; }

        /// <inheritdoc />
        /// <summary>
        ///     Runs the framework bootstrap
        /// </summary>
        public async Task RunAsync()
        {
            await RunAsync(new UnityContainer(), null);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Runs the framework bootstrap
        /// </summary>
        /// <param name="container"></param>
        public async Task RunAsync(IUnityContainer container)
        {
            await RunAsync(container, null);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Set the shell and then Runs the framework bootstrap
        /// </summary>
        /// <param name="shell"></param>
        public async Task RunAsync(IShell shell)
        {
            await RunAsync(new UnityContainer(), shell);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Bootstrapper run command with ability to provide container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="shell"></param>
        public async Task RunAsync(IUnityContainer container, IShell shell)
        {
            Container = container;

            Shell = shell;

            await InitializeAsync();
        }

        protected virtual async Task InitializeAsync()
        {
            CreateLogger();

            InitializeContainer();

            CreatePlatform();

            await CreateShellAsync();

            CreateModuleCatalog();

            await InitializeModulesAsync();

            CreateNavigationService();
        }

        /// <summary>
        ///     Create shell
        /// </summary>
        /// <returns></returns>
        protected virtual async Task CreateShellAsync()
        {
            try
            {
                Logger.Info("{0}.CreateShell", GetType().Name);

                if (Shell != null)
                {
                    Container.RegisterInstance(Shell);
                }
                else
                {
                    Shell = Container.Resolve<IShell>();
                }

                var shellViewModel = Container.Resolve<IShellViewModel>();

                await shellViewModel.InitializeAsync();

                Platform.SetBindingContext(Shell, shellViewModel);

                Shell.Bootstrapper = this;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not resolve IShell!\r\nDid you register it?", ex);
            }
        }

        protected virtual void CreateLogger()
        {
            Logger = new DefaultLogger(GetType().Name);

            Logger.Info("{0}.CreateLogger", GetType().Name);
        }

        protected virtual void CreateNavigationService()
        {
            NavigationService = Container.Resolve<INavigationService>();

            Logger.Info("{0}.CreateNavigationService", GetType().Name);
        }

        protected virtual void CreatePlatform()
        {
            Platform = Container.Resolve<IPlatform>();

            Logger.Info("{0}.CreatePlatform", GetType().Name);
        }

        protected virtual void CreateModuleCatalog()
        {
            ModuleCatalog = Container.Resolve<IModuleCatalog>();

            Logger.Info("{0}.CreateModuleCatalog", GetType().Name);
        }

        /// <summary>
        ///     Initialize all modules
        /// </summary>
        protected virtual async Task InitializeModulesAsync()
        {
            Logger.Info("{0}.InitializeModules: <modules>.Initialize()", GetType().Name);
            // Initialize each of the modules
            foreach (var module in ModuleCatalog.Modules)
            {
                await module.InitializeAsync();
            }

            // Let bootstrapper know that modules are initialized
            ModulesInitialized(ModuleCatalog);
        }

        /// <summary>
        ///     Executed by bootstrapper after modules initialized
        /// </summary>
        /// <param name="catalog"></param>
        protected virtual void ModulesInitialized(IModuleCatalog catalog)
        {
            foreach (var module in catalog.Modules)
            {
                module.ModulesInitialized(catalog);
            }

            Logger.Info("{0}.ModulesInitialized", GetType().Name);
        }

        private void InitializeContainer()
        {
            Container.RegisterType<IErrorBase, ErrorBase>()
                .RegisterType<ILoggerFacade, DefaultLogger>(new ContainerControlledLifetimeManager())
                .RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager())
                .RegisterType<IModuleCatalog, ModuleCatalog>(new ContainerControlledLifetimeManager())
                .RegisterType<IShellViewModel, ShellViewModel>(new ContainerControlledLifetimeManager())
                .RegisterType<INavigationService, NavigationService>(new ContainerControlledLifetimeManager());

            ConfigureContainer();
        }

        /// <summary>
        ///     Configure container
        /// </summary>
        protected virtual void ConfigureContainer()
        {
            Logger.Info("{0}.ConfigureContainer", GetType().Name);
        }
    }
}