#region Usings

using System;
using System.Collections.ObjectModel;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    ///     Module interface
    /// </summary>
    public interface IModule : IId<Guid>
    {
        /// <summary>
        ///     Module name
        /// </summary>
        string Name { get; set; }

        ReadOnlyCollection<IPresenter> Presenters { get; }

        /// <summary>
        ///     Initialize module
        /// </summary>
        Task InitializeAsync();

        void ModulesInitialized(IModuleCatalog moduleCatalog);
    }
}