namespace blahaj.blahaj.Player;

public class MinecraftPlayer
{
    public string Name { get; set; } = "";
    public Guid? Uuid { get; set; }

    public MinecraftPlayer() { }
    public MinecraftPlayer(string name, Guid uuid)
    {
        Name = name;
        Uuid = uuid;
    }

    public bool IsValid()
    {
        return Name.Length > 0 || Uuid != null;
    }
}