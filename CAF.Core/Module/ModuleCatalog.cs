#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using CompositeApplicationFramework.Base;
using CompositeApplicationFramework.Events;
using CompositeApplicationFramework.Interfaces;
using CompositeApplicationFramework.Types;
using Unity.Lifetime;
using Unity;
using JetBrains.Annotations;

#endregion

namespace CompositeApplicationFramework.Module
{
    /// <summary>
    ///     Hold catalog of modules
    /// </summary>
    [UsedImplicitly]
    public class ModuleCatalog : ClassBase, IModuleCatalog
    {
        private readonly Dictionary<string, IModule> _moduleCatalog;
        //public Dictionary<string, IModule> ModuleCatalogs { get { return moduleCatalog; } }

        /// <summary>
        /// </summary>
        public ModuleCatalog()
        {
            _moduleCatalog = new Dictionary<string, IModule>();
        }

        /// <summary>
        ///     Add module
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public IModuleCatalog Add(Type module)
        {
            var name = module.ToString();
            
            Add(name, module);

            return this;
        }

        /// <summary>
        ///     Add module
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        public IModuleCatalog Add(string moduleName, Type module)
        {
            if (_moduleCatalog.ContainsKey(moduleName))
            {
                Throw<Exception>("Module {0} already exists in catalog!", moduleName);
            }

            var resourceLoader = Container.Resolve<IResourceLoader>();
           
            resourceLoader.LoadResources(module);

            var moduleObj = Container.CreateChildContainer().Resolve(module);

            _moduleCatalog.Add(moduleName, (IModule) moduleObj);

            var args = new ModuleEventArgs {Module = (IModule) moduleObj, ModuleName = moduleName};

            // Publish event
            Logger.Info("PUBLISH:<ProcessEvent>(ModuleAdded) ModuleCatalog.Add: {0} ({1})", moduleObj.GetType().Name, Id);

            EventAggregator.GetEvent<ProcessEvent>()
                .Publish(new ProcessEventArgs {EventArgs = args, ProcessType = ProcessType.ModuleAdded});
            return this;
        }

        /// <summary>
        ///     Get module from catalog
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public IModule GetModule(string moduleName)
        {
            return (from catalog in _moduleCatalog where catalog.Key == moduleName select catalog.Value).FirstOrDefault();
        }

        /// <summary>
        ///     Get module list from catalog
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IModule> Modules => _moduleCatalog.Values;

        /// <summary>
        ///     Add module with lifetime manager
        /// </summary>
        /// <param name="module"></param>
        /// <param name="containerControlledLifetimeManager"></param>
        /// <returns></returns>
        public IModuleCatalog Add(Type module, IInstanceLifetimeManager containerControlledLifetimeManager)
        {
            var moduleName = module.Name;
            if (_moduleCatalog.ContainsKey(moduleName))
            {
                Throw<Exception>("Module {0} already exists in catalog!", moduleName);
            }

            var moduleObj = Container.CreateChildContainer().Resolve(module);

            // Register with lifetime manager
            Container.RegisterInstance(moduleObj, containerControlledLifetimeManager);

            _moduleCatalog.Add(moduleName, (IModule) moduleObj);

            var args = new ModuleEventArgs {Module = (IModule) moduleObj, ModuleName = moduleName};

            // Publish event
            Logger.Info("PUBLISH:<ProcessEvent>(ModuleAdded): ModuleCatalog.Add: {0} ({1})", moduleObj.GetType().Name,
                Id);

            EventAggregator.GetEvent<ProcessEvent>()
                .Publish(new ProcessEventArgs {EventArgs = args, ProcessType = ProcessType.ModuleAdded});
            return this;
        }
    }
}