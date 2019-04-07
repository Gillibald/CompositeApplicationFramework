#region Usings

using System.Threading.Tasks;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    public interface ISavable
    {
        /// <summary>
        ///     Saves this instance asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<object> SaveAsync();
    }

    public interface ISavable<T>
    {
        /// <summary>
        ///     Saves this instance asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<T> SaveAsync();
    }
}