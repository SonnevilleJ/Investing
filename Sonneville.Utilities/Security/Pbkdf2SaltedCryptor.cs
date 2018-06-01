﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace Sonneville.Utilities.Security
{
    public interface IIteratedSaltedTextHasher
    {
        byte[] DigestBytes(byte[] data, byte[] salt, string algorithm, int iterations, int digestLength);
        byte[] DigestText(string text, byte[] salt, string algorithm, int iterations, int digestLength);
    }

    public class Pbkdf2SaltedCryptor : ISaltedCryptor, IIteratedSaltedTextHasher
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
            return DigestText(message, salt, _algorithm.Name, _iterations, _algorithm.Length);
        }

        public byte[] HashBytes(byte[] message, byte[] salt)
        {
            throw new NotImplementedException();
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
