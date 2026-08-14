using System.Buffers;
using blahaj.blahaj.Stream;
using Raspite;
using Raspite.Tags;

namespace blahaj.blahaj.Registry.Data.Dimensions;

public class Dimension : IRegistryEntry
{
    public string? Identifier { get; set; }
    public void Write(MinecraftStream writer)
    {
        
        writer.WriteString(Identifier ?? "");
        var buffer = new ArrayBufferWriter<byte>();
        writer.WriteBool(false);

        /*
        CompoundTag compoundTag = AsTag();
        
        TagSerializer.Serialize(buffer, compoundTag, new TagSerializerOptions()
        {
            Network = true
        });
        writer.WriteByteArray([.. buffer.WrittenSpan]);*/
    }

    public IRegistryEntry Read(MinecraftStream writer)
    {
        throw new NotImplementedException();
    }
}