using System.Buffers;
using blahaj.blahaj.Stream;
using Raspite;
using Raspite.Tags;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.JukeboxSongs;

public class JukeboxSong : IRegistryEntry, INbt
{
   
    public int ComparatorOutput { get; set; }
    public JukeboxSongDescription Description { get; set; }
    
    public double LengthInSeconds { get; set; }
    public string SoundEvent { get; set; } 
    
    
    public string? Identifier { get; set; }
    
    // TODO Replace all descriptions with a Text Component
    
    // Todo Consolidate this all into a generic base function
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
        
        var builder = CompoundTagBuilder.Create()
            .AddInteger(ComparatorOutput, "comparator_output")
            .AddCompound(Description.AsTag(), "description")
            .AddDouble(LengthInSeconds, "length_in_seconds")
            .AddString(SoundEvent, "sound_event");

        return builder.Build();
    }
}


