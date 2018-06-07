using NUnit.Framework;
using Sonneville.Investing.Persistence.EFCore.EntityFrameworkCore;

namespace Sonneville.Investing.Persistence.IntegrationTest
{
    [TestFixture]
    public abstract class IntegrationTestsBase
    {
        protected DataContext DbContext;

        private readonly DatabaseConnectionInfo _defaultConnectionInfo = new DatabaseConnectionInfo
        {
            Hostname = "127.0.0.1",
            PortNumber = 6543,
            UserId = "postgres",
            Password = "pwd",
            Database = "investing"
        };

        protected DataContext InitializeDbContext(DatabaseConnectionInfo connectionInfo = null)
        {
            return new DataContext(connectionInfo ?? _defaultConnectionInfo);
        }

        protected void EnsureCleanDatabase()
        {
            DbContext.Database.EnsureDeleted();
            DbContext.Database.EnsureCreated();
        }

        [SetUp]
        public virtual void Setup()
        {
            DbContext = InitializeDbContext();
        }

        [TearDown]
        public void Teardown()
        {
            InitializeDbContext().Database.EnsureDeleted();
        }
    }
}
