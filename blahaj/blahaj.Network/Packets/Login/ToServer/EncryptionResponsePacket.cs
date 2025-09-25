using System.Security.Cryptography;
using System.Text;
using blahaj.blahaj.Crypto;
using blahaj.blahaj.Stream;

namespace blahaj.Network.Packets.Login;

public class EncryptionResponsePacket : Packet
{
    public byte[] SharedSecret { get; private set; }
    public byte[] VerifyToken { get; private set; }
    
    public EncryptionResponsePacket() : base(0x01)
    {
    }

    public override void Read(MinecraftStream stream)
    {
        var encryptedShared = stream.ReadPrefixedByteArray();
        SharedSecret = Encryption.Key.Decrypt(encryptedShared, RSAEncryptionPadding.Pkcs1);
        VerifyToken = stream.ReadPrefixedByteArray();
    }

    public override void Write(MinecraftStream stream)
    {
        throw new NotImplementedException();
    }
}