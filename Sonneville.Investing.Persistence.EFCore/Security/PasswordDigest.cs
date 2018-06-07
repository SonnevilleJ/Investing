using Sonneville.Investing.Persistence.EFCore.Users;

namespace Sonneville.Investing.Persistence.EFCore.Security
{
    public class PasswordDigest : Entity<long>
    {
        public long UserDatabaseId { get; set; }
        public ApplicationUser User { get; set; }
        public string Cryptor { get; set; }
        public string HashingAlgorithm { get; set; }
        public byte[] SaltUsed { get; set; }
        public int Iterations { get; set; }
        public byte[] Digest { get; set; }
    }
}
