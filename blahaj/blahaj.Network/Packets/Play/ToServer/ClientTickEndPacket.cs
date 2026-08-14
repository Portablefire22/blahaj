using blahaj.blahaj.Stream;

namespace blahaj.Network.Packets.Play.ToServer;

public class ClientTickEndPacket : Packet
{
    public ClientTickEndPacket() : base(0xD)
    {
        
    }

    public override void Read(MinecraftStream stream)
    {
        
    }

    public override void Write(MinecraftStream stream)
    {
    }
}