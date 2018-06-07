namespace Sonneville.Utilities.Security
{
    public class PasswordHash
    {
        public string CryptorName { get; set; }
        public HashAlgorithm HashAlgorithm { get; set; }
        public byte[] Salt { get; set; }
        public int Iterations { get; set; }
        public byte[] HashDigest { get; set; }
    }
}
