using Raspite.Tags;

namespace blahaj.blahaj.Registry.Data.Variants;

public abstract class Condition
{
    public string Type
    {
        get;
        set
        {
            if (ValidTypes.Contains(value))
            {
                field = value;
            }
            else
            {
                throw new ArgumentException($"Invalid SpawnCondition Type: {value}");
            }
        }
    }

    public static string[] ValidTypes { get; } =
    [
        "biome", "structure", "moon_brightness"
    ];
    
    public abstract CompoundTag AsTag();
}