using blahaj.blahaj.Entities;
using blahaj.blahaj.Network.Packets.Interfaces;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Play.ToServer;

[PacketDirection(PacketDirection.ClientToServer)]
[PacketState(ConnectionState.Play)]
public class AcceptTeleport : Packet, IInvokableWithPlayer
{
    public AcceptTeleport() : base(0)
    {
    }

    public int Id { get; set; }
    
    public override void Read(MinecraftStream stream)
    {
        Id = stream.ReadVarInt();
    }

    public override void Write(MinecraftStream stream)
    {
        stream.WriteVarInt(Id);
    }

    public Packet? Invoke()
    {
        Player.UpdateTeleport(Id);
        return null;
    }

    public MinecraftPlayer Player { get; set; }
}