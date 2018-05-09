using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Sonneville.Investing.Persistence.EFCore.EntityFrameworkCore;

namespace Sonneville.Investing.Persistence.IntegrationTest
{
    [TestFixture]
    public class MigrationTests
    {
        private DataContext _dataContext;

        [SetUp]
        public void Setup()
        {
            _dataContext = IntegrationTestConnection.GetDataContext();
        }

        [TearDown]
        public void Teardown()
        {
            _dataContext.Database.EnsureDeleted();
        }

        [Test]
        public void ShouldMigrateFromScratch()
        {
            _dataContext.Database.EnsureDeleted();
            CollectionAssert.IsEmpty(_dataContext.Database.GetAppliedMigrations());

            _dataContext.Database.Migrate();

            CollectionAssert.IsNotEmpty(_dataContext.Database.GetAppliedMigrations());
        }
    }
}
