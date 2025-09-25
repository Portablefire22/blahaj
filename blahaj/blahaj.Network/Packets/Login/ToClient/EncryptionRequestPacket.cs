using blahaj.blahaj.Crypto;
using blahaj.blahaj.Stream;

namespace blahaj.Network.Packets.Login;

public class EncryptionRequestPacket : Packet
{
    private string ServerId { get; }
    public byte[] VerifyToken { get; }
    private bool ShouldAuthenticate { get; }

    public EncryptionRequestPacket() : base(0x01) {}
    
    public EncryptionRequestPacket(string serverId, bool shouldAuth) : base(0x01)
    {
        ServerId = serverId;
        ShouldAuthenticate = shouldAuth;
        
        var dat = new byte[4];
        Rng.Random.NextBytes(dat);
        VerifyToken = dat;
    }

    public override void Read(MinecraftStream stream)
    {
        throw new NotImplementedException();
    }

    public override void Write(MinecraftStream stream)
    { 
       stream.WriteString(ServerId);
       stream.WritePrefixedByteArray(Encryption.ExportKeyAsDer());
       stream.WritePrefixedByteArray(VerifyToken);
       stream.WriteBool(ShouldAuthenticate);
    }
}