namespace blahaj.blahaj.Registry.Packs;

public static class PackTracker
{
    public static List<Pack> Known { get; private set; }

    static PackTracker()
    {
        Known = new List<Pack>();
    }

    public static void AddPack(Pack pack)
    {
        Known.Add(pack);
    } 
}