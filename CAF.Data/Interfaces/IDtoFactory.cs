#region Usings

using System;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    public interface IDtoFactory : IFactory
    {
        /// <summary>
        ///     Gets the type of the dto.
        /// </summary>
        /// <value>
        ///     The type of the dto.
        /// </value>
        Type DtoType { get; }
    }
}