using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Login.ToClient;

[PacketState(ConnectionState.Login)]
[PacketDirection(PacketDirection.ServerToClient)]
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