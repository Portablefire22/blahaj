using blahaj.blahaj.Network.Packets.Interfaces;
using blahaj.blahaj.Network.Packets.Play.ToClient;
using blahaj.blahaj.Stream;
using blahaj.blahaj.World;

namespace blahaj.blahaj.Network.Packets.Configuration.ToServer;

[PacketState(ConnectionState.Configuration)]
[PacketDirection(PacketDirection.ClientToServer)]
public class AcknowledgeFinishConfiguration : Packet, IInvokableWithClient, IInvokableWithServer
{
    public AcknowledgeFinishConfiguration() : base(0x03) {}
    public override void Read(MinecraftStream stream) {}

    public override void Write(MinecraftStream stream) {}
    public Packet? Invoke()
    {
        Client.ConnectionState = ConnectionState.Play;
        var set = WorldSettings.FromConfig(Server.Config);
        var packet = new LoginPacket(set, 1, "overworld", false, null, 
            null, 0, 0);
        return packet;
    }

    public NetClient Client { get; set; }
    public NetServer Server { get; set; }
}