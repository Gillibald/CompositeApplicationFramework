#region Usings

using CompositeApplicationFramework.Interfaces;

#endregion

namespace CompositeApplicationFramework.Base
{
    public abstract class FactoryBase<T> : IFactory<T>
    {
        /// <summary>
        /// Creates an object.
        /// </summary>
        /// <returns></returns>
        object IFactory.Create()
        {
            return Create();
        }

        /// <summary>
        /// Creates an object by criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        object IFactory.Create(object criteria)
        {
            return Create(criteria);
        }

        /// <summary>
        /// Creates the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public abstract T Create(object criteria);

        /// <summary>
        /// Creates an object by criteria.
        /// </summary>
        /// <returns></returns>
        public abstract T Create();
    }

    public abstract class FactoryBase<T, TCriteria> : FactoryBase<T>, IFactory<T, TCriteria>
    {
        /// <summary>
        /// Creates an object.
        /// </summary>
        /// <returns></returns>
        object IFactory.Create()
        {
            return Create();
        }

        /// <summary>
        /// Creates an object by specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public abstract T Create(TCriteria criteria);
    }
}