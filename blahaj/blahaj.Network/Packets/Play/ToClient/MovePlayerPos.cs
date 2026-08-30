using System.Numerics;
using blahaj.blahaj.Entities;
using blahaj.blahaj.Network.Packets.Interfaces;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Play.ToClient;

[PacketDirection(PacketDirection.ClientToServer)]
[PacketState(ConnectionState.Play)]
public class MovePlayerPos : Packet, IInvokableWithPlayer
{
    public MovePlayerPos() : base(0x1E)
    {
    }
    
    public Vector3 Position { get; set; }
    public byte Flags { get; set; }
    
    
    public override void Read(MinecraftStream stream)
    {
        var x = stream.ReadDouble();
        var y = stream.ReadDouble();
        var z = stream.ReadDouble();
        Flags =  stream.ReadUnsignedByte();
        Position = new Vector3((float)x, (float)y, (float)z);
        
    }

    public override void Write(MinecraftStream stream)
    {
        stream.WriteDouble(Position.X);
        stream.WriteDouble(Position.Y);
        stream.WriteDouble(Position.Z);
        stream.WriteUnsignedByte(Flags);
    }

    public Packet? Invoke()
    {
        Player.SetPosition(Position);
        return null;
    }

    public MinecraftPlayer Player { get; set; }
}