using Raspite.Tags;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.Variants.MobVariants.SoundVariants;

public class SoundEntry : ISoundEntry
{
    public string AmbientSound { get; set; }
    public string DeathSound { get; set; }
    public string HurtSound { get; set; }
    
    public string? StepSound { get; set; }
    
    public string? EatSound { get; set; }
    public string? GrowlSound { get; set; }
    public string? PantSound { get; set; }
    public string? WhineSound { get; set; }

    public string? BegForFoodSound { get; set; }
    public string? HissSound { get; set; }
    public string? PurrSound { get; set; }
    public string? PurreowSound { get; set; }
    public string? StrayAmbientSound { get; set; }
    
    
    public string? Type { get; set; } = null; 
    
    protected virtual CompoundTagBuilder AsTagBuilder()
    {
        var builder = CompoundTagBuilder.Create(Type ?? "")
            .AddString(AmbientSound, "ambient_sound")
            .AddString(DeathSound, "death_sound")
            .AddString(HurtSound, "hurt_sound");
        
        if  (GrowlSound != null) builder.AddString(GrowlSound, "growl_sound");
        if (PantSound != null) builder.AddString(PantSound, "pant_sound");
        if (WhineSound != null) builder.AddString(WhineSound, "whine_sound");
        if (StepSound != null) builder.AddString(StepSound, "step_sound");
        if  (EatSound != null) builder.AddString(EatSound, "eat_sound");
        
        if (BegForFoodSound != null)  builder.AddString(BegForFoodSound, "beg_for_food_sound");
        if (HissSound != null) builder.AddString(HissSound, "hiss_sound");
        if (PurrSound != null) builder.AddString(PurrSound, "purr_sound");
        if (PurreowSound != null) builder.AddString(PurreowSound, "purreow_sound");
        if (StrayAmbientSound != null) builder.AddString(StrayAmbientSound, "stray_ambient_sound");
        
        return builder;
    }

    public virtual CompoundTag AsTag()
    {
        return AsTagBuilder().Build();
    }
}