using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sonneville.Utilities.Persistence.v2
{
    public sealed class InMemoryDataStore : DataStore
    {
        private static Dictionary<Type, object> _persistedCache;

        public InMemoryDataStore()
        {
            if (_persistedCache == null)
            {
                ResetPersistedData();
                ResetCachedData();
            }
        }

        protected override T Depersist<T>()
        {
            return _persistedCache.ContainsKey(typeof(T))
                ? _persistedCache[typeof(T)] as T
                : default(T);
        }

        protected override void Persist<T>(T config)
        {
            _persistedCache[typeof(T)] = Clone(config);
        }

        protected override void ResetPersistedData()
        {
            _persistedCache = new Dictionary<Type, object>();
        }

        private static T Clone<T>(T config)
        {
            var json = JsonConvert.SerializeObject(config, JsonSerialization.Settings);
            var jObject = JObject.Parse(json);
            var jsonSerializer = JsonSerializer.Create(JsonSerialization.Settings);
            return jObject.ToObject<T>(jsonSerializer);
        }
    }
}