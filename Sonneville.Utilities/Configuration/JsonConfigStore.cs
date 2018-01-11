using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sonneville.Utilities.Configuration
{
    public class JsonConfigStore : IConfigStore
    {
        private readonly string _path;
        private readonly Dictionary<Type, object> _configs;

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new SettingsReaderContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public JsonConfigStore(string path)
        {
            _path = path;
            _configs = new Dictionary<Type, object>();
        }

        public void Save<T>(T config)
        {
            var type = typeof(T);
            if (_configs.TryGetValue(type, out var existing) && !ReferenceEquals(existing, config))
            {
                throw new ArgumentOutOfRangeException(nameof(config), config, "Must pass original config instance!");
            }

            var json = JsonConvert.SerializeObject(config, JsonSerializerSettings);

            File.WriteAllText(_path, json);
        }

        public void DeleteAll()
        {
            if (File.Exists(_path)) File.Delete(_path);
        }

        public T Load<T>() where T : class, new()
        {
            var config = FetchConfig<T>();
            _configs[typeof(T)] = config;
            return config;
        }

        private T FetchConfig<T>() where T : class, new()
        {
            return File.Exists(_path)
                ? ReadConfigFromDisk<T>()
                : (_configs.TryGetValue(typeof(T), out var existing) ? existing as T : new T());
        }

        private T ReadConfigFromDisk<T>() where T : class, new()
        {
            var jsonFile = File.ReadAllText(_path);
            return JsonConvert.DeserializeObject(jsonFile, typeof(T), JsonSerializerSettings) as T;
        }

        private class SettingsReaderContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Select(p => CreateProperty(p, memberSerialization))
                    .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Select(f => CreateProperty(f, memberSerialization)))
                    .ToList();
                props.ForEach(p =>
                {
                    p.Writable = true;
                    p.Readable = true;
                });

                return props;
            }
        }
    }
}