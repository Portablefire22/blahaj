using Raspite.Tags;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.SoundVariants;

public class CatSoundEntry : ISoundEntry
{
    public string? Type { get; set; }
    
    public string AmbientSound { get; set; }
    public string BegForFoodSound { get; set; }
    public string DeathSound { get; set; }
    public string EatSound { get; set; }
    public string HissSound { get; set; }
    public string HurtSound { get; set; }
    public string PurrSound { get; set; }
    public string PurreowSound { get; set; }
    public string StrayAmbientSound { get; set; }

    public CompoundTag AsTag()
    {
        return CompoundTagBuilder.Create(Type!)
            .AddString(AmbientSound, "ambient_sound")
            .AddString(BegForFoodSound, "beg_for_food_sound")
            .AddString(DeathSound, "death_sound")
            .AddString(EatSound, "eat_sound")
            .AddString(HissSound, "hiss_sound")
            .AddString(HurtSound, "hurt_sound")
            .AddString(PurrSound, "purr_sound")
            .AddString(PurreowSound, "purreow_sound")
            .AddString(StrayAmbientSound, "stray_ambient_sound")
            .Build();
    }
}