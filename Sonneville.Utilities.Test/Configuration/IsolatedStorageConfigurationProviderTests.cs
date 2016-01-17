using System.IO.IsolatedStorage;
using NUnit.Framework;
using Sonneville.Utilities.Configuration;
using Westwind.Utilities.Configuration;

namespace Sonneville.Utilities.Test.Configuration
{
    [TestFixture]
    public class IsolatedStorageConfigurationProviderTests
    {
        private IsolatedStorageConfigurationProvider<SampleConfigClass> _provider;
        private SampleConfigClass _config;
        private string _value1;
        private IsolatedStorageFile _store;

        // ReSharper disable once MemberCanBePrivate.Global
        // Class must be public for XML serialization to work
        public class SampleConfigClass : AppConfiguration
        {
            public string Value1 { get; set; }
        }

        [SetUp]
        public void Setup()
        {
            _value1 = "SomeConfigValue";
            _config = new SampleConfigClass();
            _config.Initialize();
            _config.Value1 = _value1;

            _store = IsolatedStorageFile.GetUserStoreForAssembly();
            _provider = new IsolatedStorageConfigurationProvider<SampleConfigClass>(_store);
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var fileName in _store.GetFileNames())
            {
                _store.DeleteFile(fileName);
            }
        }

        [Test]
        public void RoundtripTest()
        {
            Assert.IsFalse(_provider.Exists());
            var write = _provider.Write(_config);
            Assert.IsTrue(write);

            Assert.IsTrue(_provider.Exists());
            var config2 = new SampleConfigClass();
            config2.Initialize(_provider);
            Assert.AreEqual(_config.Value1, config2.Value1);
        }

        [Test]
        public void EncryptionTest()
        {
            var encryptingProvider = new IsolatedStorageConfigurationProvider<SampleConfigClass>(_store)
            {
                PropertiesToEncrypt = "Value1",
                EncryptionKey = "asdf"
            };

            var write = encryptingProvider.Write(_config);
            Assert.IsTrue(write);

            var config2 = new SampleConfigClass();
            config2.Initialize(_provider);
            Assert.AreNotEqual(_value1, config2.Value1);
        }

        [Test]
        public void DeleteRemovesConfigFile()
        {
            _provider.Write(_config);
            Assert.IsTrue(_provider.Exists());

            _provider.Delete();
            Assert.IsFalse(_provider.Exists());
            CollectionAssert.IsEmpty(_store.GetFileNames());
        }

        [Test]
        public void DeleteIsSafeToCallMultipleTimes()
        {
            _provider.Write(_config);

            _provider.Delete();
            _provider.Delete();
        }
    }
}