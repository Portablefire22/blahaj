using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Registry.Data;

public class UpdateTag
{
    public UpdateTag(string identifier, int[] entries)
    {
        Identifier = identifier;
        Entries = entries;
    }

    public string Identifier { get; set; }
    public int[] Entries { get; set; }

    public void Write(MinecraftStream stream)
    {
        stream.WriteString(Identifier);
        stream.WriteVarInt(Entries.Length);
        foreach (var entry in Entries)
        {
            stream.WriteVarInt(entry);
        }
    }
}