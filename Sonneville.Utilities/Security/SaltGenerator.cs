using System.Security.Cryptography;

namespace Sonneville.Utilities.Security
{
    public interface ISaltGenerator
    {
        byte[] GenerateSalt(int byteWidth);
    }

    public class SaltGenerator : ISaltGenerator
    {
        public byte[] GenerateSalt(int byteWidth)
        {
            var salt = new byte[byteWidth];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }
    }
}