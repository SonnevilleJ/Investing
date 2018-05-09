using Sonneville.Investing.Persistence.EFCore.EntityFrameworkCore;

namespace Sonneville.Investing.Persistence.IntegrationTest
{
    public static class IntegrationTestConnection
    {
        public static DatabaseConnectionInfo GetConnectionInfo() => new DatabaseConnectionInfo
        {
            Hostname = "127.0.0.1",
            PortNumber = 6543,
            UserId = "postgres",
            Password = "pwd",
            Database = "investing"
        };

        public static DataContext GetDataContext() => new DataContext(GetConnectionInfo());
    }
}
