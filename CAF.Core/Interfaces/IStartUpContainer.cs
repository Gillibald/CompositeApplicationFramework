#region Usings

using System;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    public interface IStartUpContainer
    {
        /// <summary>
        ///     Determines whether this instance can resolve an object of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        bool CanResolve(Type type);

        /// <summary>
        ///     Determines whether this instance can resolve an object of T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool CanResolve<T>();

        /// <summary>
        ///     Resolves an object of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        object Resolve(Type type);

        /// <summary>
        ///     Resolves an object of T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Resolve<T>();

        /// <summary>
        ///     Registers the instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        void RegisterInstance<T>(T instance);

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        object GetValue(string key);

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        T GetValue<T>(string key);

        /// <summary>
        ///     Sets the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="key">The key.</param>
        void SetValue(object value, string key);
    }
}