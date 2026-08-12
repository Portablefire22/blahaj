using blahaj.blahaj.Registry;
using blahaj.blahaj.Stream;

namespace blahaj.Network.Packets.Configuration.ToClient;

public class RegistryDataPacket : Packet
{
    public RegistryData Data { get; set; }
    
    public RegistryDataPacket(RegistryData data) : base(0x7)
    {
        Data = data;
    }

    public override void Read(MinecraftStream stream)
    {
        throw new NotImplementedException();
    }

    public override void Write(MinecraftStream stream)
    {
        Data.Write(stream);
    }
}