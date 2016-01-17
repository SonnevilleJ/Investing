using System.IO.IsolatedStorage;
using Westwind.Utilities.Configuration;

namespace Sonneville.Utilities.Configuration
{
    public interface IConfigStore
    {
        T Get<T>() where T : AppConfiguration, new();

        bool Exists<T>(T config) where T : AppConfiguration, new();

        void Erase<T>(T config) where T : AppConfiguration, new();

        void Clear();
    }

    public class ConfigStore : IConfigStore
    {
        private readonly IsolatedStorageFile _isolatedStorageFile;

        public ConfigStore(IsolatedStorageFile isolatedStorageFile)
        {
            _isolatedStorageFile = isolatedStorageFile;
        }

        public T Get<T>() where T : AppConfiguration, new()
        {
            var config = new T();
            config.Initialize(new IsolatedStorageConfigurationProvider<T>(_isolatedStorageFile));
            return config;
        }

        public bool Exists<T>(T config) where T : AppConfiguration, new()
        {
            return new IsolatedStorageConfigurationProvider<T>(_isolatedStorageFile).Exists();
        }

        public void Erase<T>(T config) where T : AppConfiguration, new()
        {
            new IsolatedStorageConfigurationProvider<T>(_isolatedStorageFile).Delete();
        }

        public void Clear()
        {
            foreach (var fileName in _isolatedStorageFile.GetFileNames())
            {
                _isolatedStorageFile.DeleteFile(fileName);
            }
        }
    }
}