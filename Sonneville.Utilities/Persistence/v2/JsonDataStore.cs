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

        protected override bool TryDepersist<T>(out object retrieved)
        {
            if (File.Exists(_path))
            {
                var json = File.ReadAllText(_path);
                var jObject = JObject.Parse(json);
                var serializedVersion = jObject[nameof(JsonMule.Version)]?.Value<string>();
                switch (serializedVersion)
                {
                    case null:
                        return LoadOldVersion<T>(out retrieved);
                    case DataStoreVersion:
                        return LoadCurrentVersion<T>(out retrieved, jObject);
                    default:
                        throw new NotSupportedException($"Unable to deserialize version {serializedVersion}!");
                }
            }

            retrieved = default(T);
            return false;
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

        private bool LoadOldVersion<T>(out object retrieved) where T : class, new()
        {
            retrieved = new JsonConfigStore<T>(_path).Load();
            return true;
        }

        private bool LoadCurrentVersion<T>(out object retrieved, JObject jObject)
        {
            var jsonSerializer = JsonSerializer.Create(JsonSerialization.Settings);
            var jsonMule = jObject.ToObject<JsonMule>(jsonSerializer);
            if (jsonMule.Cache.TryGetValue(typeof(T), out var value))
            {
                var deserialized = JsonConvert.DeserializeObject<T>(value.ToString());
                retrieved = deserialized;
                return true;
            }

            retrieved = default(T);
            return false;
        }

        private class JsonMule
        {
            public readonly string Version = DataStoreVersion;

            public readonly Dictionary<Type, object> Cache = new Dictionary<Type, object>();
        }
    }
}