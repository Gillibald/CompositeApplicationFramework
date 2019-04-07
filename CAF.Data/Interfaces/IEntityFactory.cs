#region Usings

using System;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    public interface IEntityFactory : IFactory
    {
        /// <summary>
        ///     Gets the type of the entity.
        /// </summary>
        /// <value>
        ///     The type of the entity.
        /// </value>
        Type EntityType { get; }
    }
}