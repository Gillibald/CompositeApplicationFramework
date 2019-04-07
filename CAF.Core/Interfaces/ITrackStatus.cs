using System.ComponentModel;

namespace CompositeApplicationFramework.Interfaces
{
    public interface ITrackStatus
    {
        /// <summary>
        ///     Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        [ReadOnly(true)]
        [Browsable(false)]
        bool IsDirty { get; }

        /// <summary>
        ///     Gets a value indicating whether this instance is self dirty.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is self dirty; otherwise, <c>false</c>.
        /// </value>
        [ReadOnly(true)]
        [Browsable(false)]
        bool IsSelfDirty { get; }

        /// <summary>
        ///     Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is new; otherwise, <c>false</c>.
        /// </value>
        [ReadOnly(true)]
        [Browsable(false)]
        bool IsNew { get; }

        /// <summary>
        ///     Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        [ReadOnly(true)]
        [Browsable(false)]
        bool IsValid { get; }

        /// <summary>
        ///     Gets a value indicating whether this instance is savable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is savable; otherwise, <c>false</c>.
        /// </value>
        [ReadOnly(true)]
        [Browsable(false)]
        bool IsSavable { get; }
    }
}