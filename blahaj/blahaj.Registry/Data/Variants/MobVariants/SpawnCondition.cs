using blahaj.blahaj.Registry.Data.Variants.MobVariants.Conditions;
using Raspite.Tags;
using Raspite.Tags.Building;

namespace blahaj.blahaj.Registry.Data.Variants.MobVariants;

public class SpawnCondition : INbt
{
   public int Priority { get; set; }
   public Condition? Condition { get; set; } = null;
   public CompoundTag AsTag()
   {
      var builder = CompoundTagBuilder.Create("spawn_conditions")
         .AddInteger(Priority, "priority");
      if (Condition != null) builder.AddCompound(Condition.AsTag(), "condition");
      return builder.Build();
   }
}
