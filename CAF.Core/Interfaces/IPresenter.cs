#region Usings

using System;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    ///     Presenter interface
    /// </summary>
    public interface IPresenter : IId<Guid>
    {
        /// <summary>
        ///     Presenter ViewModel
        /// </summary>
        IViewModel ViewModel { get; }

        /// <summary>
        ///     Presenter view
        /// </summary>
        object View { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        bool IsInitialized { get; }

        /// <summary>
        ///     Initialize presenter
        /// </summary>
        Task InitializeAsync();

        void ModulesInitialized(IModuleCatalog moduleCatalog);
    }
}