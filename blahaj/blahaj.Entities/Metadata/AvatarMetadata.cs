namespace blahaj.blahaj.Entities;

public class AvatarMetadata : LivingEntityMetadata
{
    public int Hand { get; set; } = 1;
    public byte DisplayedSkin { get; set; } = 127;

    protected override List<MetadataEntry> ToEntries()
    {
        var entries = base.ToEntries();

        entries.AddRange([
            new MetadataEntry(15, 42, Hand),
            new MetadataEntry(16, 0, DisplayedSkin)
        ]);
        
        return entries;
    }
}