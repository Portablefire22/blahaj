using Raspite.Tags;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.Variants;

public class BiomeCondition : Condition
{
    public BiomeCondition(string[] biomes)
    {
        Biomes = biomes;
    }

    public string[] Biomes { get; set; }
    
    public override CompoundTag AsTag()
    {
        var x = CompoundTagBuilder.Create();

        if (Biomes.Length > 1)
        {
            x.AddStringList(Biomes, "biomes");
        }
        else
        {
            x.AddString(Biomes[0], "biomes");
        }

        return x.Build();
    }
}