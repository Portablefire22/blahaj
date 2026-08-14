using System.Buffers;
using blahaj.blahaj.Stream;
using Raspite;
using Raspite.Tags;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data;

public class BannerPattern : INbt, IRegistryEntry
{
    public string AssetId { get; set; }
    public string TranslationKey { get; set; }
    
    public CompoundTag AsTag()
    {
        var builder = CompoundTagBuilder.Create()
            .AddString(AssetId, "asset_id")
            .AddString(TranslationKey, "translation_key");
        return builder.Build();
    }

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
}