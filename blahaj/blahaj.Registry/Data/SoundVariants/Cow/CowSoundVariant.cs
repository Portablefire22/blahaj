using System.Buffers;
using blahaj.blahaj.Stream;
using Raspite;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.SoundVariants.Cow;

public class CowSoundVariant : IRegistryEntry
{
    
    public CowSoundEntry Sounds { get; set; }

    public string? Identifier { get; set; }

    public void Write(MinecraftStream writer)
    {
        writer.WriteString(Identifier ?? "");
        var buffer = new ArrayBufferWriter<byte>();
        var msWriter = new TagWriter(buffer, false, true);
        writer.WriteBool(true);

        
        TagSerializer.Serialize(buffer, Sounds.AsTag(), new TagSerializerOptions()
        {
            Network = true
        });
        
        writer.WriteByteArray([.. buffer.WrittenSpan]);
    }

    public IRegistryEntry Read(MinecraftStream writer)
    {
        throw new NotImplementedException();
    }
}