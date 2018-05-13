using System.Security.Cryptography;

namespace Sonneville.Utilities.Security
{
    public class HashAlgorithm
    {
        public string Name => HashAlgorithmName.Name;
        public HashAlgorithmName HashAlgorithmName { get; }
        public int Length { get; }

        private HashAlgorithm(HashAlgorithmName hashAlgorithmName, int length)
        {
            HashAlgorithmName = hashAlgorithmName;
            Length = length;
        }

        public static readonly HashAlgorithm SHA1 = new HashAlgorithm(HashAlgorithmName.SHA1, 160);
        public static readonly HashAlgorithm SHA256 = new HashAlgorithm(HashAlgorithmName.SHA256, 256);
        public static readonly HashAlgorithm SHA384 = new HashAlgorithm(HashAlgorithmName.SHA384, 384);
        public static readonly HashAlgorithm SHA512 = new HashAlgorithm(HashAlgorithmName.SHA512, 512);
    }
}
