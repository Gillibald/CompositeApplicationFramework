#region Usings

using System;
using CompositeApplicationFramework.Types;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    public interface IChangedProperty
    {
        /// <summary>
        ///     Gets the action.
        /// </summary>
        /// <value>
        ///     The action.
        /// </value>
        ChangeAction Action { get; }

        /// <summary>
        ///     Gets the name of the property.
        /// </summary>
        /// <value>
        ///     The name of the property.
        /// </value>
        string PropertyName { get; }

        /// <summary>
        ///     Gets the new value.
        /// </summary>
        /// <value>
        ///     The new value.
        /// </value>
        object NewValue { get; }

        /// <summary>
        ///     Gets the old value.
        /// </summary>
        /// <value>
        ///     The old value.
        /// </value>
        object OldValue { get; }

        /// <summary>
        ///     Gets the type of the property.
        /// </summary>
        /// <value>
        ///     The type of the property.
        /// </value>
        Type PropertyType { get; }
    }
}