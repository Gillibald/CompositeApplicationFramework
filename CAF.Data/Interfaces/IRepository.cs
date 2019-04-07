#region Usings

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    public interface IRepository<T, in TId>
    {
        /// <summary>
        ///     Fetches all objects of type <see cref="T" /> asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> FetchAllAsync();

        /// <summary>
        ///     Fetches an object of type <see cref="T" /> by id asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<T> FetchAsync(TId id);

        /// <summary>
        ///     Inserts or updates the object of type <see cref="T" /> asynchronous.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        Task<T> InsertOrUpdateAsync(T obj);

        /// <summary>
        ///     Deletes the object of type <see cref="T" /> asynchronous.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(T obj);

        /// <summary>
        /// Updates the given field
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="id"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<T> UpdateFieldAsync<TProperty>(TId id, Expression<Func<T, TProperty>> propertyExpression, TProperty data);

        /// <summary>
        ///     Searches an object of type <see cref="T" /> by an property expression and some criteria asynchronous.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        Task<T> FindOneAsync<TProperty>(Expression<Func<T, TProperty>> propertyExpression, TProperty criteria);

        /// <summary>
        ///     Searches an object of type <see cref="T" /> by an property expression and some criteria asynchronous.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindAsync<TProperty>(Expression<Func<T, TProperty>> propertyExpression, TProperty criteria);

        /// <summary>
        ///     Creates an object of type <see cref="T" /> asynchronous.
        /// </summary>
        /// <returns></returns>
        T Create();

        /// <summary>
        /// Creates an object of type <see cref="T" /> asynchronous.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        T Create(object criteria);
    }
}