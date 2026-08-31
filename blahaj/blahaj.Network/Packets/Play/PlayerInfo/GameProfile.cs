namespace blahaj.blahaj.Network.Packets.Play.PlayerInfo;

public class GameProfile
{
    public Guid Uuid { get; set; }
    public string Username { get; set; }
    public GameProfileProperties[] Properties { get; set; }
}

public class GameProfileProperties
{
    public string Name { get; set; }
    public string Value { get; set; }
    public string? Signature { get; set; } = null;
}