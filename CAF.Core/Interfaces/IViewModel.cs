#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    ///     View model interface
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged, IDisposable, IId<Guid>
    {
        /// <summary>
        ///     Gets or sets the parent view model.
        /// </summary>
        /// <value>
        ///     The parent view model.
        /// </value>
        IViewModel ParentViewModel { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        bool IsInitialized { get; }

        /// <summary>
        ///     Initialize view model
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        ///     A hook into data that ViewModels want to share
        /// </summary>
        Dictionary<string, object> GetViewModelData(object sender, EventArgs e);
    }

    public interface IViewModel<T> : IViewModel
    {
        /// <summary>
        ///     Gets or sets the model.
        /// </summary>
        /// <value>
        ///     The model.
        /// </value>
        [Browsable(false)]
        T Model { get; set; }
    }
}