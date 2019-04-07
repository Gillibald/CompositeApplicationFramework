#region Usings

using System.Collections.Generic;
using CompositeApplicationFramework.Model;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    public interface IMenuViewModel : IViewModel
    {
        /// <summary>
        ///     Menu list
        /// </summary>
        IEnumerable<MenuEntry> MenuEntries { get; set; }

        /// <summary>
        ///     Selected menu item
        /// </summary>
        MenuEntry SelectedMenuEntry { get; set; }
    }
}