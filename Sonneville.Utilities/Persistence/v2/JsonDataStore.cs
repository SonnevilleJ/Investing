using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sonneville.Utilities.Persistence.v1;

namespace Sonneville.Utilities.Persistence.v2
{
    public class JsonDataStore : DataStore
    {
        private const string DataStoreVersion = "v2";
        private readonly string _path;
        private readonly JsonMule _jsonMule;

        public JsonDataStore(ILog log, string path)
        {
            _path = path;
            _jsonMule = new JsonMule();
            log.Debug($"Initializing with path: {path}");
        }

        protected override T Depersist<T>()
        {
            if (File.Exists(_path))
            {
                var json = File.ReadAllText(_path);
                var jObject = JObject.Parse(json);
                var serializedVersion = jObject[nameof(JsonMule.Version)]?.Value<string>();
                switch (serializedVersion)
                {
                    case null:
                        return LoadOldVersion<T>();
                    case DataStoreVersion:
                        return LoadCurrentVersion<T>(jObject);
                    default:
                        throw new NotSupportedException($"Unable to deserialize version {serializedVersion}!");
                }
            }

            return default(T);
        }

        protected override void Persist<T>(T config)
        {
            _jsonMule.Cache[typeof(T)] = config;

            var json = JsonConvert.SerializeObject(_jsonMule, JsonSerialization.Settings);
            File.WriteAllText(_path, json);
        }

        protected override void ResetPersistedData()
        {
            if (File.Exists(_path)) File.Delete(_path);
            _jsonMule.Cache.Clear();
        }

        private T LoadOldVersion<T>() where T : class, new()
        {
            return new JsonConfigStore<T>(_path).Load();
        }

        private T LoadCurrentVersion<T>(JObject jObject)
        {
            var jsonSerializer = JsonSerializer.Create(JsonSerialization.Settings);
            var jsonMule = jObject.ToObject<JsonMule>(jsonSerializer);
            return jsonMule.Cache.TryGetValue(typeof(T), out var value)
                ? JsonConvert.DeserializeObject<T>(value.ToString())
                : default(T);
        }

        private class JsonMule
        {
            public readonly string Version = DataStoreVersion;

            public readonly Dictionary<Type, object> Cache = new Dictionary<Type, object>();
        }
    }
}