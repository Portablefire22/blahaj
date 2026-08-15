using System.Text.Json.Serialization;
using Raspite.Tags;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.ArmourTrims;

public class ArmourTrimDescription : INbt
{
    [JsonPropertyName("color")]
    public string Colour { get; set; }
    public string Translate { get; set; }
    
    public CompoundTag AsTag()
    {
        var b = CompoundTagBuilder.Create("description")
            .AddString(Colour, "color")
            .AddString(Translate, "translate");
        return b.Build();
    }
}