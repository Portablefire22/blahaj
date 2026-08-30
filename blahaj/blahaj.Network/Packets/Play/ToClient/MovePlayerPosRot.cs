using System.Numerics;
using blahaj.blahaj.Entities;
using blahaj.blahaj.Network.Packets.Interfaces;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Play.ToClient;

[PacketDirection(PacketDirection.ClientToServer)]
[PacketState(ConnectionState.Play)]
public class MovePlayerPosRot : Packet, IInvokableWithPlayer
{
    public MovePlayerPosRot() : base(0x1F)
    {
    }

    public Vector3 Position { get; set; }
    public Vector2 Rotation { get; set; }
    public byte Flags { get; set; }


    public override void Read(MinecraftStream stream)
    {
        var x = stream.ReadDouble();
        var y = stream.ReadDouble();
        var z = stream.ReadDouble();
        var yaw = stream.ReadFloat();
        var pitch = stream.ReadFloat();
        Flags = stream.ReadUnsignedByte();
        Position = new Vector3((float)x, (float)y, (float)z);
        Rotation = new Vector2(yaw, pitch);
    }

    public override void Write(MinecraftStream stream)
    {
        stream.WriteDouble(Position.X);
        stream.WriteDouble(Position.Y);
        stream.WriteDouble(Position.Z);
        stream.WriteFloat(Rotation.X);
        stream.WriteFloat(Rotation.Y);
        stream.WriteUnsignedByte(Flags);
    }

    public Packet? Invoke()
    {
        Player.SetPosition(Position, rotation: Rotation);
        return null;
    }

    public MinecraftPlayer Player { get; set; }
}