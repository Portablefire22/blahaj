using blahaj.blahaj.Stream;

namespace blahaj.Network.Packets.Status;

public class PingPacket : Packet
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
}