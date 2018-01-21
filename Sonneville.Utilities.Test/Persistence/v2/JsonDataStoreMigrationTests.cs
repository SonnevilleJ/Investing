using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Sonneville.Utilities.Persistence.v1;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Utilities.Test.Persistence.v2
{
    [TestFixture]
    public class JsonDataStoreMigrationTests
    {
        private string _path;
        private JsonDataStore _store;

        [SetUp]
        public void Setup()
        {
            _path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                $"{nameof(JsonDataStoreMigrationTests)}.json"
            );
            Console.WriteLine($"Path used for tests: {_path}");
            _store = new JsonDataStore(_path);
        }

        [TearDown]
        public void Teardown()
        {
            if (File.Exists(_path))
            {
                Console.WriteLine(
                    $"Clearing persisted data... Contents follow: {Environment.NewLine}{File.ReadAllText(_path)}");
                File.Delete(_path);
            }

            Assert.False(File.Exists(_path));
        }
        
        [Test]
        public void LoadShouldUpgradeOldPersistedData()
        {
            var sampleData = CreateLegacySampleData();

            var legacyConfigStore = new JsonConfigStore(_path);
            legacyConfigStore.Save(sampleData);
            Assert.AreEqual(sampleData.A, legacyConfigStore.Get<SampleData>().A);
            
            var retrievedSampleData = _store.Get<SampleData>();
            Assert.AreEqual(sampleData.A, retrievedSampleData.A);
            Assert.AreEqual(sampleData.B, retrievedSampleData.B);
            Assert.AreEqual(sampleData.C, retrievedSampleData.C);
            Assert.AreEqual(sampleData.D, retrievedSampleData.D);
            Assert.AreEqual(sampleData.E, retrievedSampleData.E);
            Assert.AreEqual(sampleData.F, retrievedSampleData.F);
            Assert.AreEqual(sampleData.G, retrievedSampleData.G);
            Assert.AreEqual(sampleData.H, retrievedSampleData.H);
            Assert.AreEqual(sampleData.I, retrievedSampleData.I);
        }

        [Test]
        public void LoadShouldThrowIfNewerVersionDetected()
        {
            var jObject = JObject.FromObject(new {Version = "v6"});
            File.WriteAllText(_path, jObject.ToString());

            Assert.Throws<NotSupportedException>(() => _store.Load<SampleData>());
        }

        private static SampleData CreateLegacySampleData()
        {
            return new SampleData
            {
                A = "sample",
                B = 5,
                C = 500,
                D = 1234.56,
                E = 123.4567m,
                F = new TimeSpan(5, 1, 2, 3),
                G = new HashSet<string>(new[] {"backward compatibility FTW"}),
                H = new Dictionary<Type, object> {{typeof(string), "hi!"}},
                I = new List<string> {"one", "two", "three"},
            };
        }
    }
}