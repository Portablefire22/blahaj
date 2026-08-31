namespace blahaj.blahaj.Entities;

public class PlayerMetadata : AvatarMetadata
{
    public float AdditionalHearts { get; set; } = 0;
    public int Score { get; set; } = 0;
    public int? LeftShouldEntityData { get; set; } = null;
    public int? RightShouldEntityData { get; set; } = null;

    public PlayerMetadata()
    {
        Health = 20;
    }

    protected override List<MetadataEntry> ToEntries()
    {
        var entries = base.ToEntries();
        
        entries.AddRange([
            new MetadataEntry(17, 3, AdditionalHearts),
            new MetadataEntry(18, 1, Score),
            new MetadataEntry(19, 19, LeftShouldEntityData),
            new MetadataEntry(20, 19, RightShouldEntityData)
        ]);
        return entries;
    }
}