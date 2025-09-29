using blahaj.blahaj.Stream;

namespace blahaj.Network.Packets.Configuration.ToServer;

public class AcknowledgeFinishConfiguration : Packet
{
    public AcknowledgeFinishConfiguration() : base(0x03) {}
    public override void Read(MinecraftStream stream) {}

    public override void Write(MinecraftStream stream) {}
}