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
    public static class DtoFactory
    {
        private static readonly ConcurrentDictionary<Type, IFactory> FactoryCache =
            new ConcurrentDictionary<Type, IFactory>();

        static DtoFactory()
        {
            var typesWithFactory = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    typesWithFactory.AddRange(
                        assembly.GetTypes()
                            .Where(type => type.GetCustomAttributes(typeof (DtoFactoryAttribute), true).Any()));
                }
                catch (ReflectionTypeLoadException)
                {
                }
            }

            foreach (var type in typesWithFactory)
            {
                var factoryAttribute =
                    type.GetCustomAttributes(typeof (DtoFactoryAttribute), true).SingleOrDefault() as
                        DtoFactoryAttribute;

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
            return GetFactory(entityType) as T;
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

    public class DtoFactory<TDto, TEntity> : FactoryBase<TDto, TEntity>, IDtoFactory
        where TDto : class, IDto, new() where TEntity : class, IEntity
    {
        public DtoFactory()
        {
            DtoType = typeof (TDto);
        }

        public Type DtoType { get; }

        object IFactory.Create()
        {
            return Create();
        }

        public override TDto Create()
        {
            return FastActivator.CreateInstance<TDto>();
        }

        public override TDto Create(object criteria)
        {
            return Create(criteria as TEntity);
        }

        public override TDto Create(TEntity criteria)
        {
            return FastActivator<TEntity>.CreateInstance<TDto>(criteria);
        }
    }
}