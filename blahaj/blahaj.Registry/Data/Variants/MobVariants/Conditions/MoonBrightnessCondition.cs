using System.Runtime.CompilerServices;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.Variants;

public class MoonBrightnessCondition : Condition
{
    public new object Range => base.Range!;
    
    
    protected override CompoundTagBuilder AsBuilder()
    {
        var x = base.AsBuilder();
        
        switch (Range)
        {
            case double d:
                x.AddDouble(d, "range");
                break;
            case MoonBrightnessRange mbr:
            {
                var b = CompoundTagBuilder.Create("range");
            
                if (mbr.Max != null) b.AddDouble(mbr.Max, "max");
                if (mbr.Min != null) b.AddDouble(mbr.Min, "min");
                x.AddCompound(b.Build(), "range");
                break;
            }
        }

        return x;
    }
}