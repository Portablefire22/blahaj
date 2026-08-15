using System.Buffers;
using blahaj.blahaj.Registry.Data.JukeboxSongs;
using blahaj.blahaj.Stream;
using Raspite;
using Raspite.Tags;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.Instruments;

public class Instrument : IRegistryEntry, INbt
{
    public JukeboxSongDescription Description { get; set; }
    public double Range { get; set; }
    public string SoundEvent { get; set; }
    public double UseDuration { get; set; }
    public string? Identifier { get; set; }
    public void Write(MinecraftStream writer)
    {
        
        writer.WriteString(Identifier ?? "");
        var buffer = new ArrayBufferWriter<byte>();
        writer.WriteBool(true);

        CompoundTag compoundTag = AsTag();
        
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
        var builer = CompoundTagBuilder.Create()
            .AddCompound(Description.AsTag(), "description")
            .AddDouble(Range, "range")
            .AddString(SoundEvent, "sound_event")
            .AddDouble(UseDuration, "use_duration");
        return builer.Build();
    }
}