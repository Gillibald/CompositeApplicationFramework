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
using MongoDB.Driver;

#endregion

namespace CompositeApplicationFramework.DataAccess
{
    public abstract class MongoDbContext<T, TId> : ContextBase<T, TId>
        where T : IDto<TId>
    {
        protected readonly MongoDbConnector DbConnector;

        protected MongoDbContext([NotNull] MongoDbConnector dbConnector)
            : this(dbConnector, typeof(T).Name)
        {
        }

        protected MongoDbContext([NotNull] MongoDbConnector dbConnector, [NotNull] string collectionName)
        {
            DbConnector = dbConnector;

            SetCollection(collectionName);
        }

        protected IMongoCollection<T> Collection;

        public override async Task<IEnumerable<T>> FetchAllAsync()
        {
            var result = await Collection.FindAsync(_ => true);

            if (result == null) return Enumerable.Empty<T>();

            return await result.ToListAsync();
        }

        public override async Task<T> FetchAsync(TId id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);

            var result = await Collection.FindAsync(filter);

            if (result == null) return default(T);

            return await result.SingleOrDefaultAsync();
        }

        public override async Task<bool> InsertAsync(T data)
        {
            try
            {
                await Collection.InsertOneAsync(data);
            }
            catch (MongoWriteException mwx)
            {
                if (mwx.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    return false;
                }
            }
            return true;
        }

        public override async Task<bool> UpdateAsync(T data)
        {
            var filter = Builders<T>.Filter.Eq("_id", data.Id);

            var result = await Collection.ReplaceOneAsync(filter, data);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public override async Task<T> UpdateFieldAsync<TProperty>(TId id, Expression<Func<T, TProperty>> propertyExpression, TProperty data)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);

            var update = Builders<T>.Update.Set(propertyExpression, data);

            var options = new FindOneAndUpdateOptions<T> { ReturnDocument = ReturnDocument.After };

            return await Collection.FindOneAndUpdateAsync(filter, update, options);
        }

        public override async Task<bool> DeleteAsync(TId id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);

            var result = await Collection.DeleteOneAsync(filter);

            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public override async Task<T> FindOneAsync<TProperty>(Expression<Func<T, TProperty>> propertyExpression, TProperty criteria)
        {
            var filter = Builders<T>.Filter.Eq(propertyExpression.GetPropertyName(), criteria);

            var result = await Collection.FindAsync(filter);

            return await result.SingleOrDefaultAsync();
        }

        public override async Task<IEnumerable<T>> FindAsync<TProperty>(Expression<Func<T, TProperty>> propertyExpression, TProperty criteria)
        {
            var filter = Builders<T>.Filter.Eq(propertyExpression.GetPropertyName(), criteria);

            var result = await Collection.FindAsync(filter);

            return await result.ToListAsync();
        }

        private void SetCollection(string collectionName)
        {
            Collection = DbConnector.Db.GetCollection<T>(collectionName);
        }
    }
}