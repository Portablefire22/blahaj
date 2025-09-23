using blahaj.blahaj.Stream;

namespace blahaj.Network.Packets.Handshake;

public class HandshakePacket : Packet
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
        stream.WriteUnsignedByte((byte)Id);
    }
}