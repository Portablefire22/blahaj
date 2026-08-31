using System.Numerics;

namespace blahaj.blahaj.Entities;

public class LivingEntityMetadata : EntityMetadata
{
    public byte HandStates { get; set; } = 0;
    public float Health { get; set; } = 0;
    public int Particles { get; set; } = 0;
    public bool IsPotionEffectAmbient { get; set; } = false;
    public int ArrowsInEntity { get; set; } = 0;
    public int BeeStingersInEntity { get; set; } = 0;
    public Vector3? SleepingPosition { get; set; } = null;

    protected override List<MetadataEntry> ToEntries()
    {
        var entries = base.ToEntries();
        
        entries.AddRange([
            new MetadataEntry(8, 0, HandStates),
            new MetadataEntry(9, 3, Health),
            new MetadataEntry(10, 17, Particles),
            new MetadataEntry(11, 8, IsPotionEffectAmbient),
            new MetadataEntry(12, 1, ArrowsInEntity),
            new MetadataEntry(13, 1, BeeStingersInEntity),
            new MetadataEntry(14, 11, SleepingPosition)
        ]);

        return entries;
    }
}