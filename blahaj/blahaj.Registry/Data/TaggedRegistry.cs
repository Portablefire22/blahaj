using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Registry.Data;

public class TaggedRegistry
{
    public TaggedRegistry(string identifier, UpdateTag[] tags)
    {
        Identifier = identifier;
        Tags = tags;
    }

    public string Identifier { get; set; }
    public UpdateTag[] Tags { get; set; }

    public void Write(MinecraftStream stream)
    {
        stream.WriteString(Identifier);
        stream.WriteVarInt(Tags.Length);
        foreach (var tag in Tags)
        {
            tag.Write(stream);
        }
    }
}