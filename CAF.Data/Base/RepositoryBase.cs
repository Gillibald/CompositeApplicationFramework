#region Usings

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CompositeApplicationFramework.Events;
using CompositeApplicationFramework.Interfaces;
using CompositeApplicationFramework.Types;
using JetBrains.Annotations;

#endregion

namespace CompositeApplicationFramework.Base
{
    public abstract class RepositoryBase<T, TId> : IRepository<T, TId>
        where T : IEntity<TId>
    {
        protected RepositoryBase([NotNull] IEventAggregator eventAggregator, [NotNull] ILoggerFacade logger)
        {
            EventAggregator = eventAggregator;

            Logger = logger;
        }

        protected IEventAggregator EventAggregator { get; }
        protected ILoggerFacade Logger { get; set; }

        public async Task<IEnumerable<T>> FetchAllAsync()
        {
            return await FetchAllAsyncOverride();
        }

        public async Task<T> FetchAsync(TId id)
        {
            return await FetchAsyncOverride(id);
        }

        public async Task<T> InsertOrUpdateAsync([NotNull] T entity)
        {
            try
            {
                entity = await InsertOrUpdateAsyncOverride(entity);
            }
            finally
            {
                if (entity != null)
                {
                    EventAggregator.GetEvent<DataEvent<T, TId>>()
                        .Publish(new DataEventArgs<T, TId>
                        {
                            Sender = this,
                            DataType = DataType.Update,
                            Id = entity.Id,
                            Data = entity,
                            FriendlyMessage = "SaveOrUpdate",
                        });

                    Logger.Info($"{GetType()} : {typeof(T)} : {entity.Id} --> SaveOrUpdate");
                }
            }
            return entity;
        }

        public async Task<bool> DeleteAsync([NotNull] T entity)
        {
            var result = await DeleteAsyncOverride(entity);

            if (!result) return false;

            EventAggregator.GetEvent<DataEvent<T, TId>>()
                .Publish(new DataEventArgs<T, TId>
                {
                    Sender = this,
                    DataType = DataType.Delete,
                    Id = entity.Id,
                    Data = entity,
                    FriendlyMessage = "Delete"
                });

            Logger.Info($"{GetType()} : {typeof(T)} : {entity.Id} --> Delete");

            return true;
        }

        public async Task<T> UpdateFieldAsync<TProperty>(TId id, Expression<Func<T, TProperty>> propertyExpression, TProperty data)
        {
            return await UpdateFieldAsyncOverride(id, propertyExpression, data);
        }

        public async Task<T> FindOneAsync<TProperty>([NotNull] Expression<Func<T, TProperty>> propertyExpression, TProperty criteria)
        {
            return await FindOneAsyncOverride(propertyExpression, criteria);
        }

        public async Task<IEnumerable<T>> FindAsync<TProperty>([NotNull] Expression<Func<T, TProperty>> propertyExpression, TProperty criteria)
        {
            return await FindAsyncOverride(propertyExpression, criteria);
        }

        public T Create()
        {
            var entity = CreateOverride();

            Logger.Info($"{GetType()} : {typeof(T)} : {entity.Id} --> Create");

            return entity;
        }

        public T Create(object criteria)
        {
            var entity = CreateOverride(criteria);

            Logger.Info($"{GetType()} : {typeof(T)} : {entity.Id} --> Create");

            return entity;
        }

        protected abstract Task<IEnumerable<T>> FetchAllAsyncOverride();
        protected abstract Task<T> FetchAsyncOverride(TId id);
        protected abstract Task<T> InsertOrUpdateAsyncOverride([NotNull] T entity);
        protected abstract Task<bool> DeleteAsyncOverride([NotNull] T entity);
        protected abstract Task<T> UpdateFieldAsyncOverride<TProperty>(TId id, Expression<Func<T, TProperty>> propertyExpression, TProperty data);
        protected abstract Task<T> FindOneAsyncOverride<TProperty>([NotNull] Expression<Func<T, TProperty>> propertyExpression, TProperty criteria);
        protected abstract Task<IEnumerable<T>> FindAsyncOverride<TProperty>([NotNull] Expression<Func<T, TProperty>> propertyExpression, TProperty criteria);
        protected abstract T CreateOverride();
        protected abstract T CreateOverride(object criteria);
    }
}