using System.IO.IsolatedStorage;
using System.Linq;
using NUnit.Framework;
using Sonneville.Utilities.Configuration;
using Westwind.Utilities.Configuration;

namespace Sonneville.Utilities.Test.Configuration
{
    [TestFixture]
    public class ConfigStoreTests
    {
        private IsolatedStorageFile _isolatedStorageFile;
        private ConfigStore _configStore;

        [SetUp]
        public void Setup()
        {
            _isolatedStorageFile = IsolatedStorageFile.GetUserStoreForAssembly();

            ClearIsolatedStorage();

            _configStore = new ConfigStore(_isolatedStorageFile);
        }

        [TearDown]
        public void Teardown()
        {
            ClearIsolatedStorage();
            _isolatedStorageFile.Dispose();
        }

        [Test]
        public void GetSetsProvider()
        {
            var config = _configStore.Get<SampleConfigClass>();

            Assert.IsInstanceOf<SampleConfigClass>(config);
            Assert.IsInstanceOf<IsolatedStorageConfigurationProvider<SampleConfigClass>>(config.Provider);
        }

        [Test]
        public void GetReturnsRelated()
        {
            var config1 = _configStore.Get<SampleConfigClass>();
            var config2 = _configStore.Get<SampleConfigClass>();

            config1.Value1 = "alpha";
            config1.Write();
            config2.Read();
            Assert.AreEqual(config1.Value1, config2.Value1);
        }

        [Test]
        public void ConfigWritesUsingProvider()
        {
            var config = _configStore.Get<SampleConfigClass>();
            Assert.AreEqual(0, _isolatedStorageFile.GetFileNames().Count());

            config.Write();

            Assert.AreEqual(1, _isolatedStorageFile.GetFileNames().Count());
        }

        [Test]
        public void EraseDeletesSingleConfigFile()
        {
            const string filename = "ignoreme.config";
            _isolatedStorageFile.CreateFile(filename).Close();
            var config = _configStore.Get<SampleConfigClass>();
            config.Write();
            Assert.AreEqual(2, _isolatedStorageFile.GetFileNames().Count());

            _configStore.Erase(config);

            Assert.AreEqual(1, _isolatedStorageFile.GetFileNames().Count());
            CollectionAssert.Contains(_isolatedStorageFile.GetFileNames(), filename);
        }

        [Test]
        public void ClearDeletesAllConfigFiles()
        {
            const string filename = "deleteme.config";
            _isolatedStorageFile.CreateFile(filename).Close();
            CollectionAssert.Contains(_isolatedStorageFile.GetFileNames(), filename);

            _configStore.Clear();

            CollectionAssert.IsEmpty(_isolatedStorageFile.GetFileNames());
        }

        private void ClearIsolatedStorage()
        {
            foreach (var fileName in _isolatedStorageFile.GetFileNames())
            {
                _isolatedStorageFile.DeleteFile(fileName);
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        // Class must be public for XML serialization to work
        public class SampleConfigClass : AppConfiguration
        {
            public string Value1 { get; set; }
        }
    }
}