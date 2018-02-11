using NUnit.Framework;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Utilities.Test.Persistence.v2
{
    [TestFixture]
    public class InMemoryDataStoreIntegrationTests : DataStoreIntegrationTestsBase
    {
        protected override DataStore InstantiateDataStore()
        {
            return new InMemoryDataStore();
        }
    }
}