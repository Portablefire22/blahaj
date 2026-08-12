using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Registry.Data;

public interface IRegistryEntry
{
    public void Write(MinecraftStream writer);
    public IRegistryEntry Read(MinecraftStream writer);
}