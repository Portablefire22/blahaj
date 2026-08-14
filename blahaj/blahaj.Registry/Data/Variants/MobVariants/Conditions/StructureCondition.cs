using Raspite.Tags;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.Variants;

public class StructureCondition : Condition
{
    public new string[] Structure => base.Structure!;
    
    protected override CompoundTagBuilder AsBuilder()
    {
        var x = base.AsBuilder();
        
        if (Structure.Length > 1)
        {
            x.AddStringList(Structure, "structures");
        }
        else
        {
            x.AddString(Structure[0], "structures");
        }

        return x;
    }
}