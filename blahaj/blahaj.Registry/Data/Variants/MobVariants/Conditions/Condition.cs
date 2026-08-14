using System.Text.Json.Serialization;
using Raspite.Tags;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.Variants;

public class Condition
{
    public string? Type
    {
        get;
        set
        {
            if (ValidTypes.Contains(value) || value == null)
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
        "minecraft:biome", "minecraft:structure", "minecraft:moon_brightness"
    ];
    protected string[]? Biomes { get; set; }
    protected string[]? Structure { get; set; }
    
    [JsonPropertyName("Range")]
    private object? TempRange { get; set; }


    protected object? Range
    {
        get
        {
            if (TempRange == null) return null;

            if (Brightness != null)
            {
                return Brightness;
            }
            
            if (BrightnessRange != null)
            {
                return BrightnessRange;
            }
            
            switch (TempRange)
            {
                case double d:
                    Brightness = d;
                    return Brightness;
                case MoonBrightnessRange mbr:
                    BrightnessRange = mbr;
                    return BrightnessRange;
                default:
                    return null;
            }
        }
    }


    private MoonBrightnessRange? BrightnessRange { get; set; } = null;
    private double? Brightness { get; set; } = null;

    protected virtual CompoundTagBuilder AsBuilder()
    {
        var builder = CompoundTagBuilder.Create("condition");
        builder.AddString(Type, "type");
        return builder;
    }

    public virtual CompoundTag AsTag()
    {
        var builder = AsBuilder();
        return builder.Build();
    }

    public Condition AsSpecialised()
    {
        switch (Type)
        {
            case "minecraft:biome":
                return this as BiomeCondition;
            case  "minecraft:structure":
                return this as StructureCondition;
            case "minecraft:moon_brightness":
                return this as MoonBrightnessCondition;
        }
        throw new ArgumentException("Invalid Condition, could not specialise");
    }
    
}

public class MoonBrightnessRange
{
    public MoonBrightnessRange(double? max = null, double? min = null)
    {
        Max = max;
        Min = min;
    }

    public double? Max { get; } = null;
    public double? Min { get; } = null;
}
