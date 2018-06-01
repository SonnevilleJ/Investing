using System.Security.Cryptography;
using System.Text;

namespace Sonneville.Utilities.Security
{
    public class Pbkdf2SaltedCryptor : ISaltedCryptor
    {
        private readonly HashAlgorithm _algorithm;
        private readonly int _iterations;

        public Pbkdf2SaltedCryptor(HashAlgorithm algorithm, int iterations)
        {
            _algorithm = algorithm;
            _iterations = iterations;
        }

        public string Name { get; } = "PBKDF2";

        public byte[] HashString(string message, byte[] salt)
        {
            return HashBytes(Encoding.Unicode.GetBytes(message), salt);
        }

        public byte[] HashBytes(byte[] message, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(
                message,
                salt,
                _iterations,
                _algorithm.HashAlgorithmName))
                return pbkdf2.GetBytes(_algorithm.Length);
        }
    }
}
