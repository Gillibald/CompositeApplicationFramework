using System.ComponentModel;

namespace CompositeApplicationFramework.Interfaces
{
    public interface IParent
    {
        /// <summary>
        ///     Provide access to the parent reference for use
        ///     in child object code.
        /// </summary>
        /// <remarks>
        ///     This value will be Nothing for root objects.
        /// </remarks>
        [ReadOnly(true)]
        [Browsable(false)]
        IParent Parent { get; }

        /// <summary>
        ///     Override this method to be notified when a child object's
        ///     <see cref="IBusinessObject" /> state has
        ///     changed.
        /// </summary>
        /// <param name="child">The child object that was edited.</param>
        void ChildStateHasChanged(IBusinessObject child);

        /// <summary>
        ///     Sets the parent of a child.
        /// </summary>
        /// <remarks>
        ///     This value will only set once.
        /// </remarks>
        void SetParent(IParent parent);
    }
}