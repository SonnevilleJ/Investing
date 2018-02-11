using System;
using System.IO;
using log4net;
using Moq;
using NUnit.Framework;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Utilities.Test.Persistence.v2
{
    [TestFixture]
    public class JsonDataStoreRoundtripTests : DataStoreRoundtripTestsBase
    {
        private Mock<ILog> _logMock;
        private string _path;

        [SetUp]
        public override void Setup()
        {
            _path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                $"{nameof(JsonDataStoreRoundtripTests)}.json"
            );
            Console.WriteLine($"Path used for tests: {_path}");

            _logMock = new Mock<ILog>();

            base.Setup();
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

        protected override IDataStore InstantiateDataStore()
        {
            return new JsonDataStore(_logMock.Object, _path);
        }
    }
}