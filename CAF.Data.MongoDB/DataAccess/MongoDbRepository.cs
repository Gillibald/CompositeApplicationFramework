#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CompositeApplicationFramework.Base;
using CompositeApplicationFramework.Extensions;
using CompositeApplicationFramework.Interfaces;
using JetBrains.Annotations;

#endregion

namespace CompositeApplicationFramework.DataAccess
{
    public abstract class MongoDbRepository<T, TDto, TId> : RepositoryBase<T, TId>
        where T : IEntity<TId>, ISavable, IBusinessObject
        where TDto : IDto<TId>
    {
        private readonly Lazy<MongoDbContext<TDto, TId>> _contextLazy;

        protected MongoDbRepository([NotNull] MongoDbConnector connector, [NotNull] IEventAggregator eventAggregator,
            [NotNull] ILoggerFacade logger) : base(eventAggregator, logger)
        {
            Connector = connector;

            if (!Connector.Connected)
            {
                Connector.Connect();
            }

            _contextLazy = new Lazy<MongoDbContext<TDto, TId>>(CreateContext);
        }

        protected MongoDbConnector Connector { get; }
        protected abstract MongoDbContext<TDto, TId> CreateContext();

        protected override async Task<IEnumerable<T>> FetchAllAsyncOverride()
        {
            var result = await _contextLazy.Value.FetchAllAsync();

            return result?.Select(TransformDtoToEntity).ToList() ?? new List<T>();
        }

        protected override async Task<T> FetchAsyncOverride(TId id)
        {
            var result = await _contextLazy.Value.FetchAsync(id);

            if (result == null) return default(T);

            var entity = TransformDtoToEntity(result);

            return entity;
        }

        protected override async Task<T> InsertOrUpdateAsyncOverride(T entity)
        {
            if (!entity.IsSavable) return entity;

            bool result;

            var dto = TransformEntityToDto(entity);

            if (dto == null) return default(T);

            if (entity.IsNew)
            {
                result = await _contextLazy.Value.InsertAsync(dto);
            }
            else
            {
                result = await _contextLazy.Value.UpdateAsync(dto);
            }

            return result ? (T)(await entity.SaveAsync()) : default(T);
        }

        protected override async Task<bool> DeleteAsyncOverride(T entity)
        {
            return await _contextLazy.Value.DeleteAsync(entity.Id);
        }

        protected override async Task<T> FindOneAsyncOverride<TProperty>(Expression<Func<T, TProperty>> propertyExpression, TProperty criteria)
        {
            var result = await _contextLazy.Value.FindOneAsync(CreateTransformedPropertyExpression(propertyExpression), criteria);

            return result == null ? default(T) : TransformDtoToEntity(result);
        }

        protected override async Task<IEnumerable<T>> FindAsyncOverride<TProperty>(Expression<Func<T, TProperty>> propertyExpression, TProperty criteria)
        {
            var result = await _contextLazy.Value.FindAsync(CreateTransformedPropertyExpression(propertyExpression), criteria);

            var transformed = new List<T>();

            if (result == null)
            {
                return transformed;
            }

            transformed.AddRange(result.Select(TransformDtoToEntity));

            return transformed;
        }

        protected override async Task<T> UpdateFieldAsyncOverride<TProperty>(TId id, Expression<Func<T, TProperty>> propertyExpression, TProperty data)
        {
            var result = await _contextLazy.Value.UpdateFieldAsync(id, CreateTransformedPropertyExpression(propertyExpression), data);

            return TransformDtoToEntity(result);
        }

        //ToDo: Caching
        private static Expression<Func<TDto, TProperty>> CreateTransformedPropertyExpression<TProperty>(
           Expression<Func<T, TProperty>> propertyExpression)
        {
            var parameter = Expression.Parameter(typeof(TDto));

            var memberExpression = Expression.Property(parameter, propertyExpression.GetPropertyName());

            var lambdaExpression = Expression.Lambda(memberExpression, parameter);

            return (Expression<Func<TDto, TProperty>>)lambdaExpression;
        }

        protected virtual T TransformDtoToEntity([NotNull] TDto dto)
        {
            return dto.ToEntity<T>();
        }

        protected virtual TDto TransformEntityToDto([NotNull] T entity)
        {
            return entity.ToDto<TDto>();
        }
    }
}