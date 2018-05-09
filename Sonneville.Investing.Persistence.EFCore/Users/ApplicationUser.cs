namespace Sonneville.Investing.Persistence.EFCore.Users
{
    public class ApplicationUser : Entity<long>
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
