using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Play.ToServer;

[PacketState(ConnectionState.Play)]
[PacketDirection(PacketDirection.ClientToServer)]
public class ClientTickEndPacket : Packet
{
    public ClientTickEndPacket() : base(0xD)
    {
        ShouldLog = false;
    }

    public override void Read(MinecraftStream stream)
    {
        
    }

    public override void Write(MinecraftStream stream)
    {
    }
}