namespace CompositeApplicationFramework.Base
{
    #region Dependencies

    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Interfaces;
    using JetBrains.Annotations;

    #endregion

    public abstract class ContextBase<T, TId> : DisposableBase, IContext<T, TId>
        where T : IDto<TId>
    {
        protected ContextBase()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public abstract Task<IEnumerable<T>> FetchAllAsync();

        public abstract Task<T> FetchAsync(TId id);

        public abstract Task<bool> InsertAsync([NotNull] T data);

        public abstract Task<bool> UpdateAsync([NotNull] T data);

        public abstract Task<T> UpdateFieldAsync<TProperty>(TId id, Expression<Func<T, TProperty>> propertyExpression, TProperty data);

        public abstract Task<bool> DeleteAsync(TId id);

        public abstract Task<T> FindOneAsync<TProperty>(Expression<Func<T, TProperty>> propertyExpression, TProperty criteria);

        public abstract Task<IEnumerable<T>> FindAsync<TProperty>(Expression<Func<T, TProperty>> propertyExpression, TProperty criteria);
    }
}