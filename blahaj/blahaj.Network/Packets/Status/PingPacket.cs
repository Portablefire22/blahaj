using blahaj.blahaj.Network.Packets.Interfaces;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Status;

[PacketState(ConnectionState.Status)]
[PacketDirection(PacketDirection.ClientToServer, PacketDirection.ServerToClient)]
public class PingPacket : Packet, IInvokable
{
    private long Timestamp { get; set; }
    
    public PingPacket() : base(1)
    {
    }

    public override void Read(MinecraftStream stream)
    {
        Timestamp = stream.ReadLong();
    }

    public override void Write(MinecraftStream stream)
    {
        stream.WriteLong(Timestamp);
    }

    public Packet? Invoke()
    {
        return this;
    }
}