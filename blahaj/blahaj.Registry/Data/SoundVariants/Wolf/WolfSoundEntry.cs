using Raspite.Tags;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.SoundVariants;

public class WolfSoundEntry : ISoundEntry
{
    public string? Type { get; set; }
    
    public string AmbientSound { get; set; }
    public string DeathSound { get; set; }
    public string GrowlSound { get; set; }
    public string HurtSound { get; set; }
    public string PantSound { get; set; }
    public string StepSound { get; set; }
    public string WhineSound { get; set; }

    public CompoundTag AsTag()
    {
        return CompoundTagBuilder.Create(Type!)
            .AddString(AmbientSound, "ambient_sound")
            .AddString(DeathSound, "death_sound")
            .AddString(GrowlSound, "growl_sound")
            .AddString(HurtSound, "hurt_sound")
            .AddString(PantSound, "pant_sound")
            .AddString(StepSound, "step_sound")
            .AddString(WhineSound, "whine_sound")
            .Build();
    }
}