using System;
using System.Collections.Generic;

namespace Sonneville.Utilities.Persistence.v2
{
    public interface IDataStore
    {
        T Get<T>() where T : class, new();

        void Save<T>(T config);

        T Load<T>() where T : class, new();

        void DeleteAll();
    }

    public abstract class DataStore : IDataStore
    {
        private Dictionary<Type, object> _inMemoryCache = new Dictionary<Type, object>();

        public T Get<T>() where T : class, new()
        {
            return ReadFromMemoryOr(Load<T>);
        }

        public void Save<T>(T config)
        {
            var type = typeof(T);
            if (_inMemoryCache.TryGetValue(type, out var existing) && !ReferenceEquals(existing, config))
            {
                throw new ArgumentOutOfRangeException(nameof(config), config, "Must pass original config instance!");
            }

            _inMemoryCache[type] = config;

            Persist(config);
        }

        public T Load<T>() where T : class, new()
        {
            var configFromMemory = ReadFromMemoryOr(() => new T());
            var configFromPersisted = Depersist<T>();
            return Merge(configFromMemory, configFromPersisted);
        }

        public void DeleteAll()
        {
            ResetPersistedData();
            ResetCachedData();
        }

        protected void ResetCachedData()
        {
            _inMemoryCache = new Dictionary<Type, object>();
        }

        protected abstract T Depersist<T>() where T : class, new();

        protected abstract void Persist<T>(T config);

        protected abstract void ResetPersistedData();

        private T ReadFromMemoryOr<T>(Func<T> supplier) where T : class, new()
        {
            var type = typeof(T);
            if (!_inMemoryCache.ContainsKey(type))
            {
                _inMemoryCache[type] = supplier();
            }

            return _inMemoryCache[type] as T;
        }

        private static T Merge<T>(T configFromMemory, T result) where T : class, new()
        {
            if (result != null)
            {
                foreach (var propertyInfo in typeof(T).GetProperties())
                {
                    propertyInfo.SetValue(configFromMemory, propertyInfo.GetValue(result));
                }
            }

            return configFromMemory;
        }
    }
}
