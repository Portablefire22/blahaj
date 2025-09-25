using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace blahaj;

public static class MinecraftShaDigest
{
    public static string Sha(string input)
    {
        return Sha(Encoding.UTF8.GetBytes(input));
    }

    public static string Sha(byte[] input)
    {
        var hash = SHA1.HashData(input);
        Array.Reverse(hash);

        var b = new BigInteger(hash);
        if (b < 0) return "-" + (-b).ToString("x").TrimStart('0');
        return b.ToString("x").TrimStart('0');
    }
}