using System.Security.Cryptography;
using System.Text;

namespace Sonneville.Utilities.Security
{
    public interface IIteratedSaltedTextHasher
    {
        byte[] GenerateSalt(int byteWidth);
        byte[] DigestBytes(byte[] data, byte[] salt, string algorithm, int iterations, int digestLength);
        byte[] DigestText(string text, byte[] salt, string algorithm, int iterations, int digestLength);
    }

    public class Pbkdf2Cryptor : IIteratedSaltedTextHasher
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

        public byte[] DigestBytes(byte[] data, byte[] salt, string algorithm, int iterations, int digestLength)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(
                data,
                salt,
                iterations,
                new HashAlgorithmName(algorithm)))
                return pbkdf2.GetBytes(digestLength);
        }

        public byte[] DigestText(string text, byte[] salt, string algorithm, int iterations, int digestLength)
        {
            return DigestBytes(Encoding.Unicode.GetBytes(text), salt, algorithm, iterations, digestLength);
        }
    }
}
