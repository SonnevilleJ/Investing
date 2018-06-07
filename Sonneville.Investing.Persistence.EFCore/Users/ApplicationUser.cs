using Sonneville.Investing.Persistence.EFCore.Security;

namespace Sonneville.Investing.Persistence.EFCore.Users
{
    public class ApplicationUser : Entity<long>
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public PasswordDigest PasswordDigest { get; set; }
        public long PasswordDigestId { get; set; }
    }
}
