#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    /// <summary>
    ///     Module catalog interface
    /// </summary>
    public interface IModuleCatalog
    {
        /// <summary>
        ///     Add module
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        IModuleCatalog Add(Type module);

        /// <summary>
        ///     Add module
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        IModuleCatalog Add(string moduleName, Type module);

        /// <summary>
        ///     Get module from catalog
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        IModule GetModule(string moduleName);

        /// <summary>
        ///     Get module list from catalog
        /// </summary>
        /// <returns></returns>
        IEnumerable<IModule> Modules { get; }
    }
}