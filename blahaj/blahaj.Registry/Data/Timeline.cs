using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Registry.Data;

public class Timeline : IRegistryEntry
{
    public string? Identifier { get; set; }
    public void Write(MinecraftStream writer)
    {
        writer.WriteString(Identifier ?? "");
        writer.WriteBool(false);
    }

    public IRegistryEntry Read(MinecraftStream writer)
    {
        throw new NotImplementedException();
    }
}