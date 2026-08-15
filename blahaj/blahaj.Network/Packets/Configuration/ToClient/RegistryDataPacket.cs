using blahaj.blahaj.Registry;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Configuration.ToClient;

[PacketState(ConnectionState.Configuration)]
[PacketDirection(PacketDirection.ServerToClient)]
public class RegistryDataPacket : Packet
{
    public RegistryData Data { get; set; }
    
    public RegistryDataPacket(RegistryData data) : this()
    {
        Data = data;
    }

    public RegistryDataPacket() : base(0x7)
    {
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