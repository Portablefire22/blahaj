using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.Variants.MobVariants.Conditions;

public class BiomeCondition : Condition
{
    public BiomeCondition() { }

    public new string[] Biomes => base.Biomes!;
    
    protected override CompoundTagBuilder AsBuilder()
    {
        var x = base.AsBuilder();
        
        if (Biomes.Length > 1)
        {
            x.AddStringList(Biomes, "biomes");
        }
        else
        {
            x.AddString(Biomes[0], "biomes");
        }

        return x;
    }
}