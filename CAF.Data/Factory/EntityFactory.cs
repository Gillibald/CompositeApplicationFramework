#region Usings

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CompositeApplicationFramework.Attributes;
using CompositeApplicationFramework.Base;
using CompositeApplicationFramework.Interfaces;
using CompositeApplicationFramework.Utility;
using JetBrains.Annotations;

#endregion

namespace CompositeApplicationFramework.Factory
{
    public static class EntityFactory
    {
        private static readonly ConcurrentDictionary<Type, IFactory> FactoryCache =
            new ConcurrentDictionary<Type, IFactory>();

        static EntityFactory()
        {
            var typesWithFactory = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    typesWithFactory.AddRange(
                        assembly.GetTypes()
                            .Where(type => type.GetCustomAttributes(typeof (EntityFactoryAttribute), true).Any()));
                }
                catch (ReflectionTypeLoadException)
                {
                }
            }

            foreach (var type in typesWithFactory)
            {
                var factoryAttribute =
                    type.GetCustomAttributes(typeof (EntityFactoryAttribute), true).SingleOrDefault() as
                        EntityFactoryAttribute;

                if (factoryAttribute == null)
                {
                    continue;
                }

                var factory = FastActivator.CreateInstance(factoryAttribute.FactoryType) as IFactory;

                if (factory != null)
                {
                    RegisterFactory(type, factory);
                }
            }
        }

        /// <summary>
        /// Gets a Factory of type T for the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static T GetFactory<T>(Type entityType) where T : class, IFactory
        {
            return (T)GetFactory(entityType);
        }

        public static IFactory GetFactory(Type entityType)
        {
            IFactory factory;

            FactoryCache.TryGetValue(entityType, out factory);

            return factory;
        }

        public static bool RegisterFactory(Type entityType, [NotNull] IFactory factory)
        {
            return FactoryCache.TryAdd(entityType, factory);
        }
    }

    public class EntityFactory<TEntity> : FactoryBase<TEntity>, IEntityFactory where TEntity : class, IEntity, new()
    {
        public EntityFactory()
        {
            EntityType = typeof (TEntity);
        }

        public Type EntityType { get; }

        public override TEntity Create(object criteria)
        {
            return FastActivator<object>.CreateInstance<TEntity>(criteria);
        }

        public override TEntity Create()
        {
            return FastActivator.CreateInstance<TEntity>();
        }
    }

    public class EntityFactory<TEntity, TCriteria> : FactoryBase<TEntity, TCriteria>, IEntityFactory
        where TEntity : class, IEntity, new() where TCriteria : class
    {
        public EntityFactory()
        {
            EntityType = typeof (TEntity);
        }

        public Type EntityType { get; }

        public override TEntity Create(object criteria)
        {
            return Create((TCriteria) criteria);
        }

        public override TEntity Create()
        {
            return FastActivator.CreateInstance<TEntity>();
        }

        public override TEntity Create(TCriteria criteria)
        {
            return FastActivator<TCriteria>.CreateInstance<TEntity>(criteria);
        }
    }
}