using NUnit.Framework;
using Westwind.Utilities.Configuration;

namespace Sonneville.Utilities.Test
{
    [TestFixture]
    public class IsolatedStorageConfigurationProviderTests
    {
        private IsolatedStorageConfigurationProvider<SampleConfigClass> _provider;
        private SampleConfigClass _config;
        private string _value1;

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

            _provider = new IsolatedStorageConfigurationProvider<SampleConfigClass>();
        }

        [TearDown]
        public void Teardown()
        {
            _config = new SampleConfigClass();
            _config.Initialize();
            _config.Value1 = null;
            _config.Write();
        }

        [Test]
        public void RoundtripTest()
        {
            var write = _provider.Write(_config);
            Assert.IsTrue(write);

            var config2 = new SampleConfigClass();
            config2.Initialize(_provider);
            Assert.AreEqual(_config.Value1, config2.Value1);
        }

        [Test]
        public void EncryptionTest()
        {
            var encryptingProvider = new IsolatedStorageConfigurationProvider<SampleConfigClass>
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
    }
}