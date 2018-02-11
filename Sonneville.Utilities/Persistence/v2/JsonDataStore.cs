using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sonneville.Utilities.Persistence.v1;

namespace Sonneville.Utilities.Persistence.v2
{
    public class JsonDataStore : IDataStore
    {
        public const string DataStoreVersion = "v2";
        private readonly string _path;
        private readonly JsonMule _jsonMule;

        public JsonDataStore(ILog log, string path)
        {
            _path = path;
            _jsonMule = new JsonMule();
            log.Debug($"Initializing with path: {path}");
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
            _jsonMule.Cache.Clear();
        }

        public T Load<T>() where T : class, new()
        {
            var configFromCache = ReadFromCacheOr(() => new T());

            if (File.Exists(_path))
            {
                var json = File.ReadAllText(_path);
                MergePersistedData(configFromCache, json);
            }

            return configFromCache;
        }

        private void MergePersistedData<T>(T configFromCache, string json) where T : class, new()
        {
            var jObject = JObject.Parse(json);
            var serializedVersion = jObject[nameof(JsonMule.Version)]?.Value<string>();
            switch (serializedVersion)
            {
                case null:
                    MergeOldVersionData(configFromCache);
                    break;
                case DataStoreVersion:
                    MergeCurrentVersionData(jObject, configFromCache);
                    break;
                default:
                    throw new NotSupportedException($"Unable to deserialize version {serializedVersion}!");
            }
        }

        private void MergeOldVersionData<T>(T configFromCache) where T : class, new()
        {
            var oldT = new JsonConfigStore<T>(_path).Load();
            Merge(configFromCache, oldT);
        }

        private void MergeCurrentVersionData<T>(JObject jObject, T configFromCache) where T : class, new()
        {
            var jsonSerializer = JsonSerializer.Create(JsonSerialization.Settings);
            var jsonMule = jObject.ToObject<JsonMule>(jsonSerializer);
            if (jsonMule.Cache.TryGetValue(typeof(T), out var value))
            {
                var deserialized = JsonConvert.DeserializeObject<T>(value.ToString());
                Merge(configFromCache, deserialized);
            }
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