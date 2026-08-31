using System.Numerics;
using blahaj.blahaj.Entities;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Play.ToClient;

public class AddEntity : Packet
{
    public AddEntity() : base(0x1)
    {
    }

    public AddEntity(Entity entity) : this()
    {
    }

    public int EntityId { get; set; } = 0;
    public Guid EntityGuid { get; set; } = Guid.Empty;
    public int Type { get; set; } = 0;
    public Vector3 Position { get; set; } = Vector3.Zero;
    public Vector3 Velocity { get; set; } = Vector3.Zero;
    public Vector3 Rotation { get; set; } = Vector3.Zero;
    
    public int Data { get; set; }
    
    
    public override void Read(MinecraftStream stream)
    {
        throw new NotImplementedException();
    }

    public override void Write(MinecraftStream stream)
    {
        stream.WriteVarInt(EntityId);
        stream.WriteUuid(EntityGuid);
        stream.WriteVarInt(Type);
        stream.WriteDouble(Position.X);
        stream.WriteDouble(Position.Y);
        stream.WriteDouble(Position.Z);

        stream.WriteLpVec3(Velocity);
        stream.WriteUnsignedByte((byte)((Rotation.X / 180f) * 256));
        stream.WriteUnsignedByte((byte)((Rotation.Y / 180f) * 256));
        stream.WriteUnsignedByte((byte)((Rotation.Z / 180f) * 256));
        stream.WriteVarInt(Data);
    }
}