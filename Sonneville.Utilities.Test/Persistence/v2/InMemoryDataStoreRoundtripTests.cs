using NUnit.Framework;
using Sonneville.Utilities.Persistence.v2;

namespace Sonneville.Utilities.Test.Persistence.v2
{
    [TestFixture]
    public class InMemoryDataStoreRoundtripTests : DataStoreRoundtripTestsBase
    {
        protected override IDataStore InstantiateDataStore()
        {
            return new InMemoryDataStore();
        }
    }
}