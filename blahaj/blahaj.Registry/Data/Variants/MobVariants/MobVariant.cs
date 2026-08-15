using System.Buffers;
using blahaj.blahaj.Stream;
using Raspite;
using Raspite.Tags;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.Variants.MobVariants;

public class MobVariant : IRegistryEntry, INbt
{
    public string? Identifier { get; set; }
    public string AssetId { get; set; }
    public string? BabyAssetId { get; set; }
    public string? Model { get; set; }
    public MobVariantAssets? Assets { get; set; }
    public MobVariantAssets? BabyAssets { get; set; }
    public SpawnCondition[] SpawnConditions { get; set; }
    
    
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
            .AddString(AssetId, "asset_id");
        if (BabyAssetId != null) builder.AddString(BabyAssetId, "baby_asset_id");
        if (Model != null) builder.AddString(Model, "model");
        if (Assets != null)
        {
            Assets.Type = "assets";
            builder.AddCompound(Assets.AsTag(), "assets");
        }
        if (BabyAssets != null)
        {
            BabyAssets.Type = "baby_assets";
            builder.AddCompound(BabyAssets.AsTag(), "baby_assets");
        }

        var cond = new List<CompoundTag>();
        foreach (var condition in SpawnConditions)
        {
            cond.Add(condition.AsTag());
        }

        return builder.Build();
    }

}

public class MobVariantAssets : INbt
{
    public string? Type { get; set; }
    
    public string Angry { get; set; }
    public string Wild { get; set; }
    public string Tame { get; set; }
    public CompoundTag AsTag()
    {
        var builder = CompoundTagBuilder.Create(Type!)
            .AddString(Angry, "angry")
            .AddString(Wild, "wild")
            .AddString(Tame, "tame");
        return builder.Build();
    }
}