using System.Linq;
using Sonneville.Utilities.Security;

namespace Sonneville.Fidelity.Shell.Authentication
{
    public interface IPasswordService
    {
        PasswordHash HashPassword(string password);
        bool ValidatePassword(string password, PasswordHash currentHash);
    }

    public class PasswordService : IPasswordService
    {
        private readonly ISaltGenerator _saltGenerator;
        private readonly ISaltedCryptor _saltedCryptor;
        private readonly HashAlgorithm _hashAlgorithm;
        private readonly int _iterations;

        public PasswordService(
            ISaltGenerator saltGenerator,
            ISaltedCryptor saltedCryptor)
        {
            _saltGenerator = saltGenerator;
            _saltedCryptor = saltedCryptor;
            _hashAlgorithm = saltedCryptor.Algorithm;
            _iterations = saltedCryptor.Iterations;
        }

        public PasswordHash HashPassword(string password)
        {
            return HashPassword(
                password,
                _saltedCryptor,
                _hashAlgorithm,
                _iterations,
                _saltGenerator.GenerateSalt(_hashAlgorithm.Length));
        }

        public bool ValidatePassword(string password, PasswordHash currentHash)
        {
            return HashPassword(
                    password,
                    _saltedCryptor,
                    currentHash.HashAlgorithm,
                    currentHash.Iterations,
                    currentHash.Salt)
                .HashDigest
                .SequenceEqual(currentHash.HashDigest);
        }

        private static PasswordHash HashPassword(
            string password,
            ISaltedCryptor saltedCryptor,
            HashAlgorithm hashAlgorithm,
            int iterations,
            byte[] salt)
        {
            return new PasswordHash
            {
                CryptorName = saltedCryptor.Name,
                HashAlgorithm = hashAlgorithm,
                Salt = salt,
                Iterations = iterations,
                HashDigest = saltedCryptor.HashString(password, salt),
            };
        }
    }
}