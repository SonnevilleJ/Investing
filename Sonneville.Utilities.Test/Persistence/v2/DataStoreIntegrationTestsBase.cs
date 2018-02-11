using System;
using NUnit.Framework;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Utilities.Test.Persistence.v2
{
    [TestFixture]
    public abstract class DataStoreIntegrationTestsBase
    {
        protected IDataStore Store;

        protected abstract DataStore InstantiateDataStore();

        [SetUp]
        public virtual void Setup()
        {
            Store = InstantiateDataStore();
        }

        [Test]
        public void GetShouldLoadIfNotCached()
        {
            Store.Save(new SampleData
            {
                A = "original",
            });

            var configStore = InstantiateDataStore();
            var one = configStore.Get<SampleData>();
            var two = configStore.Get<SampleData>();

            Assert.AreEqual("original", one.A);
            Assert.AreEqual("original", two.A);
            Assert.AreSame(one, two);
        }

        [Test]
        public void GetShouldNotLoadAndReturnCachedIfCached()
        {
            var data = new SampleData
            {
                A = "original",
            };
            Store.Save(data);
            data.A = "changed";

            var result = Store.Get<SampleData>();
            Assert.AreEqual("changed", result.A);
            Assert.AreSame(data, result);
        }

        [Test]
        public void GetShouldReturnDefaultIfNotPersisted()
        {
            Store.Save(new SampleData
            {
                A = "original",
            });

            var configStore = InstantiateDataStore();
            var data = configStore.Get<OtherData>();

            AssertDefaultConfig(data);
        }

        [Test]
        public void LoadShouldReturnDefaultIfNotPersisted()
        {
            Store.Save(new SampleData
            {
                A = "original",
            });

            var configStore = InstantiateDataStore();
            var data = configStore.Load<OtherData>();

            AssertDefaultConfig(data);
        }

        [Test]
        public void LoadShouldUpdateAndReturnPersistedConfig()
        {
            var data = new SampleData
            {
                A = "original",
            };
            Store.Save(data);
            data.A = "changed";

            var result = Store.Load<SampleData>();
            Assert.AreEqual("original", result.A);
            Assert.AreSame(data, result);
        }

        [Test]
        public void LoadShouldReturnDefaultConfigdWhenNoFilePresent()
        {
            var data = Store.Load<SampleData>();

            AssertDefaultConfig(data);
        }

        [Test]
        public void DeleteAllShouldEmptyCache()
        {
            var data = new SampleData
            {
                A = "original",
            };
            Store.Save(data);

            Store.DeleteAll();

            AssertDefaultConfig(Store.Get<SampleData>());
        }

        [Test]
        public void SaveShouldThrowIfSavingDifferentConfigOfSameType()
        {
            Store.Get<SampleData>();

            var imposter = new SampleData();
            Assert.Throws<ArgumentOutOfRangeException>(() => Store.Save(imposter));
        }

        private static void AssertDefaultConfig<T>(T data)
        {
            Assert.NotNull(data);
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                var propertyType = propertyInfo.PropertyType;
                var defaultValue = propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
                var actualValue = propertyInfo.GetValue(data);
                Assert.AreEqual(defaultValue, actualValue);
            }
        }
    }
}