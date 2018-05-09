using Microsoft.EntityFrameworkCore.Design;

namespace Sonneville.Investing.Persistence.EFCore.EntityFrameworkCore
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var databaseConnectionInfo = new DatabaseConnectionInfo
            {
                Hostname = "127.0.0.1",
                PortNumber = 6543,
                UserId = "postgres",
                Password = "pwd",
                Database = "investing"
            };
            return new DataContext(databaseConnectionInfo);
        }
    }
}