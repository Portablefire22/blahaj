using System.Buffers;
using blahaj.blahaj.Stream;
using Raspite;
using Raspite.Tags;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.Variants;

public class PaintingVariant : INbt, IRegistryEntry
{
    
    public string AssetId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    
    
    public CompoundTag AsTag()
    {
        var b = CompoundTagBuilder.Create()
            .AddString(AssetId, "asset_id")
            .AddInteger(Width, "width")
            .AddInteger(Height, "height")
            .Build();
        return b;
    }

    public string Identifier { get; set; }
    public void Write(MinecraftStream writer)
    {
        writer.WriteString(Identifier ?? "");
        var buffer = new ArrayBufferWriter<byte>();
        writer.WriteBool(false);
        return;

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
}