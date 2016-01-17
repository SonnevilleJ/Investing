using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using Westwind.Utilities.Configuration;

namespace Sonneville.Utilities.Configuration
{
    public class IsolatedStorageConfigurationProvider<T> : StringConfigurationProvider<T>
        where T : AppConfiguration, new()
    {
        private readonly IsolatedStorageFile _store;
        private readonly string _path;

        public IsolatedStorageConfigurationProvider(IsolatedStorageFile store)
        {
            _store = store;
            _path = $"{typeof (T).FullName}.config";
        }

        public override bool Read(AppConfiguration config)
        {
            try
            {
                if (_store.FileExists(_path))
                {
                    byte[] bytes;
                    using (var fileStream = _store.OpenFile(_path, FileMode.Open))
                    {
                        bytes = new byte[fileStream.Length];
                        fileStream.Read(bytes, 0, bytes.Length);
                    }
                    config.Read(GetDefaultEncoding().GetString(bytes));
                }

                return true;
            }
            finally
            {
                DecryptFields(config);
            }
        }

        public override bool Write(AppConfiguration config)
        {
            try
            {
                EncryptFields(config);
                using (var fileStream = _store.OpenFile(_path, FileMode.Create))
                {
                    var bytes = GetDefaultEncoding().GetBytes(config.WriteAsString());
                    fileStream.Write(bytes, 0, bytes.Length);
                }
                return true;
            }
            finally
            {
                DecryptFields(config);
            }
        }

        public bool Exists()
        {
            return _store.FileExists(_path);
        }

        public void Delete()
        {
            if (Exists()) _store.DeleteFile(_path);
        }

        private static Encoding GetDefaultEncoding()
        {
            return Encoding.UTF8;
        }
    }
}