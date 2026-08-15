using blahaj.blahaj.Network.Packets.Interfaces;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Handshake.ToServer;

[PacketState(ConnectionState.Handshake)]
[PacketDirection(PacketDirection.ClientToServer)]
public class HandshakePacket : Packet, IInvokableWithClient
{
    public int ProtocolVersion { get; private set; }
    public string ServerAddress { get; private set; }
    public ushort ServerPort { get; private set; }
    public ConnectionState Intent { get; private set; }
    
    public HandshakePacket() : base(0x00){}
    
    public override void Read(MinecraftStream stream)
    {
        ProtocolVersion = stream.ReadVarInt();
        ServerAddress = stream.ReadString();
        ServerPort = stream.ReadUnsignedShort();
        Intent = (ConnectionState) stream.ReadVarInt();
    }

    public override void Write(MinecraftStream stream)
    {
    }

    public Packet? Invoke()
    {
        Client.ConnectionState = Intent;
        return null;
    }

    public NetClient Client { get; set; }
}