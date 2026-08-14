using System.Buffers;
using blahaj.blahaj.Stream;
using Raspite;
using Raspite.Tags;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data;

public class ArmourTrim : IRegistryEntry, INbt
{
    
    public string? Identifier { get; set; }
    
    public string AssetName { get; set; }
    public ArmourTrimDescription Description { get; set; }
    public Dictionary<string, string>? OverrideArmorAssets { get; set; }
    
    
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
            .AddString(AssetName, "asset_name")
            .AddCompound(Description.AsTag(), "description");

        if (OverrideArmorAssets == null) return builder.Build();
        
        var x = CompoundTagBuilder.Create("override_armor_assets");
        foreach (var (key,value) in OverrideArmorAssets)
        {
            x.AddString(value, key);
        }
        builder.AddCompound(x.Build(), "override_armor_assets");
        return builder.Build();
    }
}
