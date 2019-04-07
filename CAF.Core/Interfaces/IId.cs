using System.ComponentModel;

namespace CompositeApplicationFramework.Interfaces
{
    public interface IId<out T>
    {
        /// <summary>
        ///     Component Id
        /// </summary>
        [ReadOnly(true)]
        [Browsable(true)]
        T Id { get; }
    }
}