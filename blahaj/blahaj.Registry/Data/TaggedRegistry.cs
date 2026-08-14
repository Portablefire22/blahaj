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
        using var ms = new MemoryStream();
        using var msWriter = new MinecraftStream(ms);
        msWriter.WriteString(Identifier);
        msWriter.WriteVarInt(Tags.Length);
        foreach (var tag in Tags)
        {
            tag.Write(msWriter);
        }
        var str = "";
        foreach (var b in ms.ToArray())
        {
            str += $"{b} ";
        }
        Console.WriteLine(str);
        stream.WriteByteArray(ms.ToArray());
    }
}