using System.Numerics;
using blahaj.blahaj.Entities;
using blahaj.blahaj.Network.Packets.Interfaces;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Play.ToClient;

[PacketDirection(PacketDirection.ClientToServer)]
[PacketState(ConnectionState.Play)]
public class MovePlayerRot : Packet, IInvokableWithPlayer
{
    public MovePlayerRot() : base(0x20)
    {
    }

    public Vector2 Rotation { get; set; }
    public Byte Flags { get; set; }
    
    public override void Read(MinecraftStream stream)
    {
        var yaw = stream.ReadFloat();
        var pitch = stream.ReadFloat();
        Flags = stream.ReadUnsignedByte();
        Rotation = new Vector2(yaw, pitch);
    }

    public override void Write(MinecraftStream stream)
    {
        stream.WriteFloat(Rotation.X);
        stream.WriteFloat(Rotation.Y);
        stream.WriteUnsignedByte(Flags);
    }

    public Packet? Invoke()
    {
        Player.SetPosition(rotation: Rotation);

        return null;
    }

    public MinecraftPlayer Player { get; set; }
}