using blahaj.blahaj.Stream;

namespace blahaj.Network.Packets.Login;

public class LoginAcknowledgedPacket : Packet
{
    public LoginAcknowledgedPacket() : base(0x03)
    {
    }

    public override void Read(MinecraftStream stream)
    {
    }

    public override void Write(MinecraftStream stream)
    {
    }
}