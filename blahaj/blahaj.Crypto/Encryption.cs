using System.Buffers.Text;
using System.Security.Cryptography;

namespace blahaj.blahaj.Crypto;

public static class Encryption
{
    public static RSA Key { get; private set; }

    static Encryption()
    {
        Key = RSA.Create(1024);
        Key.ExportParameters(true);
    }

    public static byte[] ExportKeyAsDer()
    {
        return Key.ExportSubjectPublicKeyInfo();
    }
}