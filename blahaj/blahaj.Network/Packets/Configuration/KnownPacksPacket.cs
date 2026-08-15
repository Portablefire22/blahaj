using blahaj.blahaj.Network.Packets.Interfaces;
using blahaj.blahaj.Registry.Packs;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Configuration;


[PacketState(ConnectionState.Configuration)]
[PacketDirection(PacketDirection.ClientToServer, PacketDirection.ServerToClient)]
public class KnownPacksPacket : Packet, IInvokableWithClient
{
    public Pack[] KnownPacks { get; private set; }
    
    public KnownPacksPacket() : base(0x07) {}

    public KnownPacksPacket(Pack[] packs) : base(0x07, 0x0E)
    {
        KnownPacks = packs;
    }
    public override void Read(MinecraftStream stream)
    {
        var length = stream.ReadVarInt();
        KnownPacks = new Pack[length];
        for (var i = 0; i < length; i++)
        {
            var name = stream.ReadString();
            var id = stream.ReadString();
            var version = stream.ReadString();
            KnownPacks[i] = new Pack(name, id, version);
        }
    }

    public override void Write(MinecraftStream stream)
    {
       stream.WriteVarInt(KnownPacks.Length);
       foreach (var pack in KnownPacks)
       {
           stream.WriteString(pack.Namespace);
           stream.WriteString(pack.Id);
           stream.WriteString(pack.Version);
       }
    }

    public Packet? Invoke()
    {
        Client.KnownPacks = KnownPacks;
        Client.SendRegistryData();
        return null;
    }

    public NetClient Client { get; set; }
}