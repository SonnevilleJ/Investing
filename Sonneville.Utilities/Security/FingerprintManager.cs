using System.Linq;

namespace Sonneville.Utilities.Security
{
    public interface IFingerprintManager
    {
        Fingerprint HashPassword(string password);
        bool VerifyPassword(string password, Fingerprint fingerprint);
    }

    public class FingerprintManager : IFingerprintManager
    {
        private readonly IIteratedSaltedTextHasher _hasher;
        private readonly HashAlgorithm _hashAlgorithm;
        private readonly int _iterations;

        public FingerprintManager(IIteratedSaltedTextHasher hasher, HashAlgorithm hashAlgorithm, int iterations)
        {
            _hasher = hasher;
            _hashAlgorithm = hashAlgorithm;
            _iterations = iterations;
        }

        public Fingerprint HashPassword(string password)
        {
            var salt = _hasher.GenerateSalt(_hashAlgorithm.Length);
            var digest = _hasher.DigestText(
                password,
                salt,
                _hashAlgorithm.Name,
                _iterations,
                _hashAlgorithm.Length
            );
            return new Fingerprint
            {
                Algorithm = _hashAlgorithm.Name,
                Iterations = _iterations,
                Salt = salt,
                Digest = digest,
            };
        }

        public bool VerifyPassword(string password, Fingerprint fingerprint)
        {
            var digestText = _hasher.DigestText(
                password,
                fingerprint.Salt,
                fingerprint.Algorithm,
                fingerprint.Iterations,
                fingerprint.Digest.Length
            );
            return fingerprint.Digest.SequenceEqual(digestText);
        }
    }
}
