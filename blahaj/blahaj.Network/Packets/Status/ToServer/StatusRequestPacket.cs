using blahaj.blahaj.Stream;

namespace blahaj.Network.Packets.Status;

public class StatusRequestPacket : Packet
{
    public StatusRequestPacket() : base(0){}
    
    public override void Read(MinecraftStream stream)
    {
        // Has no data
    }
    
    public override void Write(MinecraftStream stream)
    {
    }
}