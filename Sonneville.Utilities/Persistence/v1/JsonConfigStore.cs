using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Sonneville.Utilities.Persistence.v1
{
    public class JsonConfigStore<T> : IConfigStore<T> where T : class, new()
    {
        private readonly string _path;
        private readonly Dictionary<Type, object> _configs = new Dictionary<Type, object>();

        public JsonConfigStore(string path)
        {
            _path = path;
        }

        public T Get()
        {
            var config = ReadFromCacheOrLoad();
            _configs[typeof(T)] = config;
            return config;
        }

        public void Save(T config)
        {
            var type = typeof(T);
            if (_configs.TryGetValue(type, out var existing) && !ReferenceEquals(existing, config))
            {
                throw new ArgumentOutOfRangeException(nameof(config), config, "Must pass original config instance!");
            }

            _configs[typeof(T)] = config;

            var json = JsonConvert.SerializeObject(config, JsonSerialization.Settings);
            File.WriteAllText(_path, json);
        }

        public void DeleteAll()
        {
            if (File.Exists(_path)) File.Delete(_path);
        }

        public T Load()
        {
            var configFromCache = ReadFromCacheOrNew();

            if (File.Exists(_path))
            {
                var jsonFile = File.ReadAllText(_path);
                var configFromDisk = JsonConvert.DeserializeObject<T>(jsonFile, JsonSerialization.Settings);
                foreach (var propertyInfo in typeof(T).GetProperties())
                {
                    propertyInfo.SetValue(configFromCache, propertyInfo.GetValue(configFromDisk));
                }
            }

            return configFromCache;
        }

        private T ReadFromCacheOrNew()
        {
            return _configs.TryGetValue(typeof(T), out var existing)
                ? existing as T
                : new T();
        }

        private T ReadFromCacheOrLoad()
        {
            return _configs.TryGetValue(typeof(T), out var existing)
                ? existing as T
                : Load();
        }
    }
}