﻿namespace Sonneville.Utilities.Security
{
    public interface ISaltedCryptor
    {
        string Name { get; }
        byte[] HashString(string message, byte[] salt);
        byte[] HashBytes(byte[] message, byte[] salt);
    }
}
