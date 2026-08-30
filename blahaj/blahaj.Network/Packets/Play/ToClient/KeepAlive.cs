using blahaj.blahaj.Entities;
using blahaj.blahaj.Network.Packets.Interfaces;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Play.ToClient;

[PacketDirection(PacketDirection.ServerToClient, PacketDirection.ClientToServer)]
[PacketState(ConnectionState.Play)]
public class KeepAlive : Packet, IInvokableWithPlayer
{
    public KeepAlive() : base(0x1c, 0x2c) { }

    public KeepAlive(long id) : this()
    {
        Id = id;
    }

    public long Id { get; set; }
    
    public override void Read(MinecraftStream stream)
    {
        Id = stream.ReadLong();
    }

    public override void Write(MinecraftStream stream)
    {
        stream.WriteLong(Id);
    }

    public Packet? Invoke()
    {
        Player.UpdateKeepAlive(Id);
        return null;
    }

    public MinecraftPlayer Player { get; set; }
}