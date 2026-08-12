using System.Buffers;
using System.Collections.Immutable;
using blahaj.blahaj.Stream;
using Raspite;
using Raspite.Tags;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.SoundVariants;

public class WolfSoundVariant : IRegistryEntry
{
    
    public WolfSoundEntry AdultSounds { get; set; }
    public WolfSoundEntry BabySounds { get; set; }


    public string? Identifier { get; set; }

    public void Write(MinecraftStream writer)
    {
        writer.WriteString(Identifier ?? "");
        var buffer = new ArrayBufferWriter<byte>();
        var msWriter = new TagWriter(buffer, false, true);
        writer.WriteBool(true);

        var x = CompoundTagBuilder.Create()
            .AddCompound(AdultSounds.AsTag(), "adult_sounds")
            .AddCompound(BabySounds.AsTag(), "baby_sounds").Build();
        
        TagSerializer.Serialize(buffer, x, new TagSerializerOptions()
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