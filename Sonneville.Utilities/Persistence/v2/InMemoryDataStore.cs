using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sonneville.Utilities.Persistence.v2
{
    public class InMemoryDataStore : IDataStore
    {
        private static Dictionary<Type, object> _persistedCache;
        private Dictionary<Type, object> _inMemoryCache = new Dictionary<Type, object>();

        public InMemoryDataStore()
        {
            if (_persistedCache == null)
            {
                DeleteAll();
            }
        }

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
            
            var json = JsonConvert.SerializeObject(config, JsonSerialization.Settings);
            var jObject = JObject.Parse(json);
            var jsonSerializer = JsonSerializer.Create(JsonSerialization.Settings);
            var clone = jObject.ToObject<T>(jsonSerializer);

            _persistedCache[type] = clone;
        }

        public T Load<T>() where T : class, new()
        {
            var configFromMemory = ReadFromMemoryOr(() => new T());
            var configFromPersisted = _persistedCache.TryGetValue(typeof(T), out var retrieved) ? retrieved as T : configFromMemory;
            return Merge(configFromMemory, configFromPersisted);
        }

        public void DeleteAll()
        {
            _persistedCache = new Dictionary<Type, object>();
            _inMemoryCache = new Dictionary<Type, object>();
        }

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
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                propertyInfo.SetValue(configFromMemory, propertyInfo.GetValue(result));
            }

            return configFromMemory;
        }
    }
}