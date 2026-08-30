using System.Numerics;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Play.ToClient;

[PacketDirection(PacketDirection.ClientToServer)]
[PacketState(ConnectionState.Play)]
public class SynchronizePlayerPosition : Packet
{
    public SynchronizePlayerPosition() : base (0x48) { }

    public SynchronizePlayerPosition(int teleportId, Vector3 position, Vector3 velocity, 
        Vector2 rotation, TeleportFlags flags) : this()
    {
        TeleportId = teleportId;
        X = position.X;
        Y = position.Y;
        Z = position.Z;
        VelocityX = velocity.X;
        VelocityY = velocity.Y;
        VelocityZ = velocity.Z;
        Yaw = rotation.X;
        Pitch = rotation.Y;
        Flags = flags;
    }

    public int TeleportId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public double VelocityX { get; set; }
    public double VelocityY { get; set; }
    public double VelocityZ { get; set; }
    public float Yaw { get; set; }
    public float Pitch { get; set; }
    
    public TeleportFlags Flags { get; set; }
    
    public override void Read(MinecraftStream stream)
    {
        throw new NotImplementedException();
    }

    public override void Write(MinecraftStream stream)
    {
        stream.WriteVarInt(TeleportId);
        stream.WriteDouble(X);
        stream.WriteDouble(Y);
        stream.WriteDouble(Z);
        stream.WriteDouble(VelocityX);
        stream.WriteDouble(VelocityY);
        stream.WriteDouble(VelocityZ);
        stream.WriteFloat(Yaw);
        stream.WriteFloat(Pitch);
        stream.WriteInt((int)Flags);
    }
}

[Flags]
public enum TeleportFlags : int
{
    RelativeX = 0x1,
    RelativeY = 0x2,
    RelativeZ = 0x4,
    RelativeYaw = 0x8,
    RelativePitch = 0x10,
    RelativeVelocityX  = 0x20,
    RelativeVelocityY   = 0x40,
    RelativeVelocityZ   = 0x80,
    RotateVelocity = 0x100,
}