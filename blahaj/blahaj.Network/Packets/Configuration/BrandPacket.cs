using System.Text;
using blahaj.blahaj.Stream;

namespace blahaj.Network.Packets.Configuration;

public class BrandPacket : Packet
{
    public string Channel { get; private set; }
    
    public BrandPacket() : base(0x02, 0x01)
    {
    }

    public BrandPacket(string channel) : base(0x02, 0x01)
    {
        Channel = channel;
    }

    public override void Read(MinecraftStream stream)
    {
        stream.ReadString(); // minecraft:brand
        Channel = stream.ReadString();
    }

    public override void Write(MinecraftStream stream)
    {
        stream.WriteString("minecraft:brand");
        stream.WriteString(Channel);
    }
}