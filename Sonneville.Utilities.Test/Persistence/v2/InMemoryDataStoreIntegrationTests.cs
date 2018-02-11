using NUnit.Framework;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Utilities.Test.Persistence.v2
{
    [TestFixture]
    public class InMemoryDataStoreIntegrationTests : DataStoreIntegrationTestsBase
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            Store.DeleteAll();
        }

        protected override DataStore InstantiateDataStore()
        {
            return new InMemoryDataStore();
        }
    }
}