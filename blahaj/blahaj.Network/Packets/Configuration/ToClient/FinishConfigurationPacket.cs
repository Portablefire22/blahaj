using blahaj.blahaj.Stream;
using Org.BouncyCastle.Utilities.Encoders;

namespace blahaj.Network.Packets.Configuration.ToClient;

public class FinishConfigurationPacket : Packet
{
    public FinishConfigurationPacket() : base (0x03) {}

    // No data
    public override void Read(MinecraftStream stream) {}

    // No data
    public override void Write(MinecraftStream stream) {}
}