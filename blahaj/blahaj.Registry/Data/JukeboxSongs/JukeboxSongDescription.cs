using Raspite.Tags;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.JukeboxSongs;

public class JukeboxSongDescription : INbt
{
    public string Translate { get; set; }
    public CompoundTag AsTag()
    {
        var x = CompoundTagBuilder.Create("description")
            .AddString(Translate, "translate");
        return x.Build();
    }
}