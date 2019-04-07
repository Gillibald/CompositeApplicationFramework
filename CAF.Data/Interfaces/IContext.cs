#region Usings

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

#endregion

namespace CompositeApplicationFramework.Interfaces
{
    public interface IContext<T, in TId> : IDisposable, IId<Guid>
    {
        /// <summary>
        ///     Fetches all objects asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> FetchAllAsync();

        /// <summary>
        ///     Fetches an object by id asynchronously.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<T> FetchAsync(TId id);

        /// <summary>
        ///     Inserts the an object asynchronously.
        /// </summary>
        /// <param name="data">The entity.</param>
        /// <returns></returns>
        Task<bool> InsertAsync(T data);

        /// <summary>
        ///     Updates an object asynchronously.
        /// </summary>
        /// <param name="data">The entity.</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T data);

        /// <summary>
        ///     Deletes an object asynchronously
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(TId id);

        /// <summary>
        /// Updates a given field
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
        ///     Searches objects of type <see cref="T" /> by an property expression and some criteria asynchronous.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindAsync<TProperty>(Expression<Func<T, TProperty>> propertyExpression, TProperty criteria);     
    }
}