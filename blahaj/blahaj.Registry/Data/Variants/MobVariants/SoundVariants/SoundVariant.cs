using System.Buffers;
using blahaj.blahaj.Stream;
using Raspite;
using Raspite.Tags;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.Variants.MobVariants.SoundVariants;

public class SoundVariant : IRegistryEntry, INbt
{
    public string? Identifier { get; set; }
   
    public SoundEntry? Sounds { get; set; }
    
    public SoundEntry? AdultSounds { get; set; }
    public SoundEntry? BabySounds { get; set; }
    
    public void Write(MinecraftStream writer)
    {
        writer.WriteString(Identifier ?? "");
        var buffer = new ArrayBufferWriter<byte>();
        writer.WriteBool(true);

        var compoundTag = AsTag();
        
        TagSerializer.Serialize(buffer, compoundTag, new TagSerializerOptions()
        {
            Network = true
        });
    
        writer.WriteByteArray([.. buffer.WrittenSpan]);
    }

    public IRegistryEntry Read(MinecraftStream writer)
    {
        throw new NotImplementedException();
    }

    public CompoundTag AsTag()
    {
        CompoundTag compoundTag;
        if (AdultSounds is not null && BabySounds is not null)
        {
            AdultSounds.Type = "adult_sounds";
            BabySounds.Type = "baby_sounds";
            compoundTag = CompoundTagBuilder.Create()
                .AddCompound(AdultSounds.AsTag(), "adult_sounds")
                .AddCompound(BabySounds.AsTag(), "baby_sounds").Build();
        }
        else 
        {
            compoundTag = Sounds!.AsTag();
        }

        return compoundTag;
    }
}