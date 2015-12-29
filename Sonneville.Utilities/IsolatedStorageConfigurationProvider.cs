using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using Westwind.Utilities.Configuration;

namespace Sonneville.Utilities
{
    public class IsolatedStorageConfigurationProvider<T> : StringConfigurationProvider<T> where T : AppConfiguration, new()
    {
        public override bool Read(AppConfiguration config)
        {
            try
            {
                var userStore = GetUserStore();
                var path = $"{typeof(T).FullName}.config";

                if (userStore.FileExists(path))
                {
                    byte[] bytes;
                    using (var fileStream = userStore.OpenFile(path, FileMode.Open))
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
                var userStore = GetUserStore();
                var path = $"{typeof(T).FullName}.config";
                using (var fileStream = userStore.OpenFile(path, FileMode.Create))
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

        private static IsolatedStorageFile GetUserStore()
        {
            return IsolatedStorageFile.GetUserStoreForAssembly();
        }

        private static Encoding GetDefaultEncoding()
        {
            return Encoding.UTF8;
        }
    }
}
