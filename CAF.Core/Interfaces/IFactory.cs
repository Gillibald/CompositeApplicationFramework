namespace CompositeApplicationFramework.Interfaces
{
    public interface IFactory
    {
        /// <summary>
        ///     Creates an object.
        /// </summary>
        /// <returns></returns>
        object Create();

        /// <summary>
        ///     Creates an object by criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        object Create(object criteria);
    }

    public interface IFactory<out T> : IFactory
    {
        /// <summary>
        ///     Creates an object.
        /// </summary>
        /// <returns></returns>
        new T Create();

        /// <summary>
        /// Creates an object by criteria.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        new T Create(object criteria);
    }

    public interface IFactory<out T, in TCriteria> : IFactory<T>
    {
        /// <summary>
        ///  Creates an object by criteria
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        T Create(TCriteria criteria);
    }
}