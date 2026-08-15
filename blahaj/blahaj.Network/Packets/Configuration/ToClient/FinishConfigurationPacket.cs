using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Configuration.ToClient;

[PacketState(ConnectionState.Configuration)]
[PacketDirection(PacketDirection.ServerToClient)]
public class FinishConfigurationPacket : Packet
{
    public FinishConfigurationPacket() : base (0x03) {}

    // No data
    public override void Read(MinecraftStream stream) {}

    // No data
    public override void Write(MinecraftStream stream) {}
}