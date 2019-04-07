#region Usings

using System.Windows.Controls;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    /// <summary>
    ///     View model holder
    /// </summary>
    public interface IViewModelHolder
    {
        /// <summary>
        ///     Gets the view model holder.
        /// </summary>
        /// <value>The view model holder.</value>
        ItemsControl Holder { get; }
    }
}