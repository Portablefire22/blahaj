using blahaj.blahaj.Registry.Data;
using blahaj.blahaj.Stream;
using Raspite.Tags;

namespace blahaj.Network.Packets.Configuration.ToClient;

public class UpdateTagsPacket : Packet
{
    public UpdateTagsPacket() : base(0xD)
    {
    }

    public UpdateTagsPacket(TaggedRegistry[] tags) : this()
    {
        Tags = tags;
    }
    
    public TaggedRegistry[] Tags { get; set; }
    
    public override void Read(MinecraftStream stream)
    {
        throw new NotImplementedException();
    }

    public override void Write(MinecraftStream stream)
    {
        stream.WriteVarInt(Tags.Length);
        foreach (var tag in Tags)
        {
            tag.Write(stream);
        }
    }
}