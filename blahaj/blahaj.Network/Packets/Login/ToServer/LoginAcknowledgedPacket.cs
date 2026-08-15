using blahaj.blahaj.Network.Packets.Interfaces;
using blahaj.blahaj.Registry.Packs;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Login.ToServer;

[PacketState(ConnectionState.Login)]
[PacketDirection(PacketDirection.ClientToServer)]
public class LoginAcknowledgedPacket : Packet, IInvokableWithClient
{
    public LoginAcknowledgedPacket() : base(0x03)
    {
    }

    public override void Read(MinecraftStream stream)
    {
    }

    public override void Write(MinecraftStream stream)
    {
    }

    public Packet? Invoke()
    {
        Client.ConnectionState = ConnectionState.Configuration;
        Client.SendKnownPacks();
        return null;
    }

    public NetClient Client { get; set; }
}