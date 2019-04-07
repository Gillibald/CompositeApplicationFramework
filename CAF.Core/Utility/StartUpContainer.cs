#region Usings

using System;
using System.Collections.Concurrent;
using CompositeApplicationFramework.Interfaces;
using JetBrains.Annotations;
using Unity;

#endregion

namespace CompositeApplicationFramework.Utility
{
    public class StartUpContainer : IStartUpContainer
    {
        private readonly IUnityContainer _container = new UnityContainer();
        private readonly ConcurrentDictionary<string, object> _valueStore = new ConcurrentDictionary<string, object>();

        public StartUpContainer()
        {
        }

        public StartUpContainer([NotNull] IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            _container = container;
        }

        public bool CanResolve(Type type)
        {
            return _container.IsRegistered(type);
        }

        public bool CanResolve<T>()
        {
            return _container.IsRegistered<T>();
        }

        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public void RegisterInstance<T>(T instance)
        {
            _container.RegisterInstance(instance);
        }

        public object GetValue(string key)
        {
            object value;

            _valueStore.TryGetValue(key, out value);

            return value;
        }

        public T GetValue<T>(string key)
        {
            object value;

            _valueStore.TryGetValue(key, out value);

            if (value is T)
            {
                return (T) value;
            }

            return default(T);
        }

        public void SetValue(object value, string key)
        {
            _valueStore.TryAdd(key, value);
        }
    }
}