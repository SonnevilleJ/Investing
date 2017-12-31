using System;
using System.IO;
using System.Linq;
using Nini.Config;

namespace Sonneville.Utilities.Configuration
{
    public interface INiniConfigStore
    {
        void Save<T>(T config);

        T Read<T>() where T : new();

        void DeleteAll();
    }

    public class NiniConfigStore : INiniConfigStore
    {
        private readonly string _path;

        public NiniConfigStore(string path)
        {
            _path = path;
        }

        public void Save<T>(T config)
        {
            if (!File.Exists(_path)) File.Create(_path).Close();
            var configSource = new IniConfigSource(_path);

            var section = configSource.Configs[GetSectionForType<T>()] ?? configSource.AddConfig(GetSectionForType<T>());

            typeof(T).GetProperties()
                .ToDictionary(info => info.Name, info => info.GetValue(config))
                .Where(kvp => kvp.Value != null).ToList()
                .ForEach(kvp => section.Set(kvp.Key, kvp.Value));
            configSource.Save();
        }

        public T Read<T>() where T : new()
        {
            var config = new T();
            if (File.Exists(_path))
            {
                var configSource = new IniConfigSource(_path);
                foreach (var propertyInfo in typeof(T).GetProperties())
                {
                    var stringValue = configSource.Configs[GetSectionForType<T>()].Get(propertyInfo.Name);
                    dynamic value = Convert.ChangeType(stringValue, propertyInfo.PropertyType);
                    propertyInfo.SetValue(config, value);
                }
            }

            return config;
        }

        public void DeleteAll()
        {
            if (File.Exists(_path)) File.Delete(_path);
        }

        private static string GetSectionForType<T>()
        {
            return typeof(T).FullName;
        }
    }
}
