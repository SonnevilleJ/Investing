using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Sonneville.Utilities.Security
{
    public class HashAlgorithm
    {
        public string Name => HashAlgorithmName.Name;
        public HashAlgorithmName HashAlgorithmName { get; }
        public int Length { get; }

        private static readonly IDictionary<HashAlgorithmName, int> Algorithms = new Dictionary<HashAlgorithmName, int>
        {
            {HashAlgorithmName.SHA1, 160},
            {HashAlgorithmName.SHA256, 256},
            {HashAlgorithmName.SHA384, 384},
            {HashAlgorithmName.SHA512, 512},
        };

        private HashAlgorithm(HashAlgorithmName hashAlgorithmName)
        {
            HashAlgorithmName = hashAlgorithmName;
            Length = Algorithms[hashAlgorithmName];
        }

        public static readonly HashAlgorithm SHA1 = new HashAlgorithm(HashAlgorithmName.SHA1);
        public static readonly HashAlgorithm SHA256 = new HashAlgorithm(HashAlgorithmName.SHA256);
        public static readonly HashAlgorithm SHA384 = new HashAlgorithm(HashAlgorithmName.SHA384);
        public static readonly HashAlgorithm SHA512 = new HashAlgorithm(HashAlgorithmName.SHA512);

        public static HashAlgorithm Parse(string algorithm)
        {
            var hashAlgorithmName = Algorithms.Keys.FirstOrDefault(han => han.Name == algorithm);

            if (hashAlgorithmName == default(HashAlgorithmName))
            {
                throw new NotSupportedException($"Unsupported algorithm: {algorithm}");
            }

            return new HashAlgorithm(hashAlgorithmName);
        }
    }
}