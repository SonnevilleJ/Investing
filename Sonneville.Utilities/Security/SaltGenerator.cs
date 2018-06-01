using System.Security.Cryptography;

namespace Sonneville.Utilities.Security
{
    public interface ISaltGenerator
    {
        byte[] GenerateSalt2(int byteWidth);
    }

    public class SaltGenerator : ISaltGenerator
    {
        public byte[] GenerateSalt2(int byteWidth)
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