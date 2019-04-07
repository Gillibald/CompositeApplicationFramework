#region Usings

using System;
using CompositeApplicationFramework.Interfaces;

#endregion

namespace CompositeApplicationFramework.Events
{
    /// <summary>
    ///     Module event arguments
    /// </summary>
    public class ModuleEventArgs : EventArgs
    {
        /// <summary>
        ///     Module name
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        ///     Module reference
        /// </summary>
        public IModule Module { get; set; }
    }
}