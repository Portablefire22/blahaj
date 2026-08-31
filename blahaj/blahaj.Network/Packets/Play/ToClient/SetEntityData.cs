using blahaj.blahaj.Entities;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Play.ToClient;

public class SetEntityData : Packet
{
    public SetEntityData() : base(0x63)
    {
    }

    public SetEntityData(int id, EntityMetadata metadata) : this()
    {
        EntityId = id;
        Metadata = metadata;
    }

    public int EntityId { get; set; }
    public EntityMetadata Metadata { get; set; }
    
    public override void Read(MinecraftStream stream)
    {
        throw new NotImplementedException();
    }

    public override void Write(MinecraftStream stream)
    {
        stream.WriteVarInt(EntityId);
        Metadata.Write(stream);
    }
}