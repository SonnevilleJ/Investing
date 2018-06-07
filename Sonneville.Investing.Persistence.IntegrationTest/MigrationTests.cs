using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Sonneville.Investing.Persistence.IntegrationTest
{
    [TestFixture]
    public class MigrationTests : IntegrationTestsBase
    {
        [Test]
        public void ShouldMigrateFromScratch()
        {
            DbContext.Database.EnsureDeleted();
            CollectionAssert.IsEmpty(DbContext.Database.GetAppliedMigrations());

            DbContext.Database.Migrate();

            CollectionAssert.IsNotEmpty(DbContext.Database.GetAppliedMigrations());
        }
    }
}
