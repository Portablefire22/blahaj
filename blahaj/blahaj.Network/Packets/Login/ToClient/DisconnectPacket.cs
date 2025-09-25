using blahaj.blahaj.Stream;

namespace blahaj.Network.Packets.Login;

public class DisconnectPacket : Packet
{
    public string Reason { get; private set; }
    public DisconnectPacket() : base(0)
    {
    }

    public override void Read(MinecraftStream stream)
    {
        stream.ReadJsonTextComponent();
    }

    public override void Write(MinecraftStream stream)
    {
        stream.WriteJsonTextComponent();
    }
}