using blahaj.blahaj.Stream;

namespace blahaj.Network.Packets.Play.ToClient;

public class LoginPacket : Packet
{
    public LoginPacket() :  base(0x2B) {}
    public override void Read(MinecraftStream stream)
    {
        throw new NotImplementedException();
    }

    public override void Write(MinecraftStream stream)
    {
        throw new NotImplementedException();
    }
}