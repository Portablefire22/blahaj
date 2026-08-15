using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Configuration.ToServer;

[PacketState(ConnectionState.Configuration)]
[PacketDirection(PacketDirection.ClientToServer)]
public class ClientInformationPacket : Packet
{
    public ClientInformationPacket() : base(0x00) {}

    public string Locale { get; private set; }
    public sbyte ViewDistance { get; private set; }
    public int ChatMode { get; private set; }
    public bool ChatColours { get; private set; }
    public byte DisplayedSkinParts { get; private set; }
    public int MainHand { get; private set; }
    public bool EnableTextFiltering { get; private set; }
    public bool AllowServerListings { get; private set; }
    public int ParticleStatus { get; private set; }
    public override void Read(MinecraftStream stream)
    {
        Locale = stream.ReadString();
        ViewDistance = stream.ReadByte();
        ChatMode = stream.ReadVarInt();
        ChatColours = stream.ReadBool();
        DisplayedSkinParts = stream.ReadUnsignedByte();
        MainHand = stream.ReadVarInt();
        EnableTextFiltering = stream.ReadBool();
        AllowServerListings = stream.ReadBool();
        ParticleStatus = stream.ReadVarInt();
    }

    public override void Write(MinecraftStream stream)
    {
        throw new NotImplementedException();
    }
}