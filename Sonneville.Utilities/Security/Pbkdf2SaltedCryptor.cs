using System.Security.Cryptography;
using System.Text;

namespace Sonneville.Utilities.Security
{
    public class Pbkdf2SaltedCryptor : ISaltedCryptor
    {
        public HashAlgorithm Algorithm { get; }

        public int Iterations { get; }

        public Pbkdf2SaltedCryptor(HashAlgorithm algorithm, int iterations)
        {
            Algorithm = algorithm;
            Iterations = iterations;
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
                Iterations,
                Algorithm.HashAlgorithmName))
                return pbkdf2.GetBytes(Algorithm.Length);
        }
    }
}
