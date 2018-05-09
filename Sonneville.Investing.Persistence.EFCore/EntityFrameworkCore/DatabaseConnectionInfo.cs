namespace Sonneville.Investing.Persistence.EFCore.EntityFrameworkCore
{
    public class DatabaseConnectionInfo
    {
        public string Hostname { get; set; }
        public int PortNumber { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }

        public string ConnectionString => $"Server={Hostname};"
                                          + $"Port={PortNumber};"
                                          + $"User Id={UserId};"
                                          + $"Password={Password};"
                                          + $"Database={Database};";
    }
}
