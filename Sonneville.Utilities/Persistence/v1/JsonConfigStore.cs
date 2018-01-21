using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sonneville.Utilities.Persistence.v1
{
    public class JsonConfigStore : IConfigStore
    {
        private readonly string _path;
        private readonly Dictionary<Type, object> _configs = new Dictionary<Type, object>();

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new SettingsReaderContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public JsonConfigStore(string path)
        {
            _path = path;
        }

        public T Get<T>() where T : class, new()
        {
            var config = ReadFromCacheOrLoad<T>();
            _configs[typeof(T)] = config;
            return config;
        }

        public void Save<T>(T config)
        {
            var type = typeof(T);
            if (_configs.TryGetValue(type, out var existing) && !ReferenceEquals(existing, config))
            {
                throw new ArgumentOutOfRangeException(nameof(config), config, "Must pass original config instance!");
            }

            _configs[typeof(T)] = config;

            var json = JsonConvert.SerializeObject(config, JsonSerializerSettings);
            File.WriteAllText(_path, json);
        }

        public void DeleteAll()
        {
            if (File.Exists(_path)) File.Delete(_path);
        }

        public T Load<T>() where T : class, new()
        {
            var configFromCache = ReadFromCacheOrNew<T>();

            if (File.Exists(_path))
            {
                var jsonFile = File.ReadAllText(_path);
                var configFromDisk = JsonConvert.DeserializeObject(jsonFile, typeof(T), JsonSerializerSettings) as T;
                foreach (var propertyInfo in typeof(T).GetProperties())
                {
                    propertyInfo.SetValue(configFromCache, propertyInfo.GetValue(configFromDisk));
                }
            }

            return configFromCache;
        }

        private T ReadFromCacheOrNew<T>() where T : class, new()
        {
            return _configs.TryGetValue(typeof(T), out var existing)
                ? existing as T
                : new T();
        }

        private T ReadFromCacheOrLoad<T>() where T : class, new()
        {
            return _configs.TryGetValue(typeof(T), out var existing)
                ? existing as T
                : Load<T>();
        }

        private class SettingsReaderContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

                return type.GetProperties(bindingFlags)
                    .Select(propertyInfo => CreateProperty(propertyInfo, memberSerialization))
                    .Union(type.GetFields(bindingFlags).Select(field => CreateProperty(field, memberSerialization)))
                    .Select(p =>
                    {
                        p.Writable = true;
                        p.Readable = true;
                        return p;
                    })
                    .ToList();
            }
        }
    }
}