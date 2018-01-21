using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Sonneville.Utilities.Persistence.v2
{
    public class JsonDataStore : IDataStore
    {
        private readonly string _path;
        private readonly JsonMule _jsonMule;

        public JsonDataStore(string path)
        {
            _path = path;
            _jsonMule = new JsonMule();
        }

        public T Get<T>() where T : class, new()
        {
            var config = ReadFromCacheOr(Load<T>);
            _jsonMule.Cache[typeof(T)] = config;
            return config;
        }

        public void Save<T>(T config)
        {
            var type = typeof(T);
            if (_jsonMule.Cache.TryGetValue(type, out var existing) && !ReferenceEquals(existing, config))
            {
                throw new ArgumentOutOfRangeException(nameof(config), config, "Must pass original config instance!");
            }

            _jsonMule.Cache[typeof(T)] = config;

            var json = JsonConvert.SerializeObject(_jsonMule, JsonSerialization.Settings);
            File.WriteAllText(_path, json);
        }

        public void DeleteAll()
        {
            if (File.Exists(_path)) File.Delete(_path);
        }

        public T Load<T>() where T : class, new()
        {
            Console.WriteLine($"Trying to load {typeof(T)}");
            var configFromCache = ReadFromCacheOr(() => new T());

            if (File.Exists(_path))
            {
                var jsonFile = File.ReadAllText(_path);
                var jsonMule = JsonConvert.DeserializeObject<JsonMule>(jsonFile, JsonSerialization.Settings);
                foreach (var propertyInfo in typeof(T).GetProperties())
                {
                    var jsonMuleObject = jsonMule.Cache[typeof(T)];
                    var result = JsonConvert.DeserializeObject<T>(jsonMuleObject.ToString());
                    propertyInfo.SetValue(configFromCache, propertyInfo.GetValue(result));
                }
            }

            return configFromCache;
        }

        private T ReadFromCacheOr<T>(Func<T> instantiator) where T : class, new()
        {
            return _jsonMule.Cache.TryGetValue(typeof(T), out var existing)
                ? existing as T
                : instantiator();
        }

        private class JsonMule
        {
            public readonly Dictionary<Type, object> Cache = new Dictionary<Type, object>();
        }
    }
}