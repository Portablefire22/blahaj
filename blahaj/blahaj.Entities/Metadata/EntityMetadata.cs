using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Entities;

public class EntityMetadata
{
   public byte Status { get; set; } = 0;
   public int AirTicks { get; set; } = 300;
   public object? CustomName { get; set; } = null;
   public bool IsCustomNameVisible { get; set; } = false;
   public bool IsSilent { get; set; } = false;
   public bool HasNoGravity { get; set; } = false;
   public int Pose { get; set; } = 0;
   public int TicksFrozenInPowderSnow { get; set; } = 0;

   
   
   protected virtual List<MetadataEntry> ToEntries()
   {
      var entries = new List<MetadataEntry>();
      
      entries.AddRange([
         new MetadataEntry(0, 0, Status),
         new MetadataEntry(1, 1, AirTicks),
         new MetadataEntry(2, 6, null),
         new MetadataEntry(3, 8, IsCustomNameVisible),
         new MetadataEntry(4, 8, IsSilent),
         new MetadataEntry(5, 8, HasNoGravity),
         new MetadataEntry(6, 20, Pose),
         new MetadataEntry(7, 1, TicksFrozenInPowderSnow)
      ]);

      return entries;
   }
   
   public virtual void Write(MinecraftStream stream)
   {
      var entries = ToEntries();
      
      entries.Add(new MetadataEntry(0xFF, 0, 0));

      foreach (var entry in entries)
      {
         entry.Write(stream);
      }
   }
   
}
