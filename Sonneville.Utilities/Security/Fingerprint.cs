namespace Sonneville.Utilities.Security
{
    public class Fingerprint
    {
        public byte[] Digest { get; set; }
        public byte[] Salt { get; set; }
        public string Algorithm { get; set; }
        public int Iterations { get; set; }
    }
}
