using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sonneville.Utilities.Persistence.v1;

namespace Sonneville.Utilities.Persistence.v2
{
    public class JsonDataStore : IDataStore
    {
        private const string DataStoreVersion = "v2";
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
            var configFromCache = ReadFromCacheOr(() => new T());

            if (File.Exists(_path))
            {
                var json = File.ReadAllText(_path);
                var jObject = JObject.Parse(json);
                var serializedVersion = jObject[nameof(JsonMule.Version)]?.Value<string>();
                if (serializedVersion == null)
                {
                    var oldT = new JsonConfigStore(_path).Load<T>();
                    Merge(configFromCache, oldT);
                }
                else if (serializedVersion == DataStoreVersion)
                {
                    var jsonSerializer = JsonSerializer.Create(JsonSerialization.Settings);
                    var jsonMule = jObject.ToObject<JsonMule>(jsonSerializer);
                    var result = JsonConvert.DeserializeObject<T>(jsonMule.Cache[typeof(T)].ToString());
                    Merge(configFromCache, result);
                }
                else
                {
                    throw new NotSupportedException($"Unable to deserialize version {serializedVersion}!");
                }
            }

            return configFromCache;
        }

        private static void Merge<T>(T configFromCache, T result) where T : class, new()
        {
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                propertyInfo.SetValue(configFromCache, propertyInfo.GetValue(result));
            }
        }

        private T ReadFromCacheOr<T>(Func<T> supplier) where T : class, new()
        {
            return _jsonMule.Cache.TryGetValue(typeof(T), out var existing)
                ? existing as T
                : supplier();
        }

        private class JsonMule
        {
            public readonly string Version = DataStoreVersion;

            public readonly Dictionary<Type, object> Cache = new Dictionary<Type, object>();
        }
    }
}