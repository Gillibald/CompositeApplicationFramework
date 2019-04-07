#region Usings

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CompositeApplicationFramework.Interfaces;
using Unity.Lifetime;
using Unity;

#endregion

namespace CompositeApplicationFramework.Base
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    /// <summary>
    ///     Module baseclass
    /// </summary>
    public abstract class ModuleBase : ClassBase, IModule
    {
        private readonly IList<IPresenter> _presenters = new List<IPresenter>();

        /// <summary>
        ///     Module base constructor
        /// </summary>
        protected ModuleBase()
        {
            Presenters = new ReadOnlyCollection<IPresenter>(_presenters);
        }

        /// <summary>
        ///     Module presenters
        /// </summary>
        [Browsable(false)]
        public ReadOnlyCollection<IPresenter> Presenters { get; }

        /// <summary>
        ///     Module Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Platform
        /// </summary>
        [Dependency]
        public IPlatform Platform { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Initialize module
        /// </summary>
        public virtual async Task InitializeAsync()
        {
            await RegisterTypesAsync();
            await InitializeViewsAsync();
        }

        /// <summary>
        ///     Add presenter to Presenters collection
        /// </summary>
        /// <param name="presenter"></param>
        protected async Task AddPresenterAsync(IPresenter presenter)
        {
            // We want all presenters to be disposed when application
            // exits - it can manage components it needs to dispose.
            Container.RegisterInstance(presenter, new ContainerControlledLifetimeManager());

            // Add the presenter to the Presenters collection
            _presenters.Add(presenter);

            // Initialize the presenter and its view model
            await presenter.InitializeAsync();
        }

        /// <summary>
        ///     Register Views  (called by Initialize)
        /// </summary>
        protected virtual Task InitializeViewsAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Register types (called by Initialize)
        /// </summary>
        protected virtual Task RegisterTypesAsync()
        {
            return Task.CompletedTask;
        }

        void IModule.ModulesInitialized(IModuleCatalog moduleCatalog)
        {
            ModulesInitialized(moduleCatalog);
        }

        protected virtual void ModulesInitialized(IModuleCatalog moduleCatalog)
        {
            foreach (var presenter in Presenters)
            {
                presenter.ModulesInitialized(moduleCatalog);
            }
        }

        /// <summary>
        ///     Get presenter from Presenters collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetPresenter<T>() where T : IPresenter
        {
            return (T)Presenters.FirstOrDefault(p => p is T);
        }
    }
}