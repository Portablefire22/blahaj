using blahaj.Network.Packets.Configuration.ToServer;
using blahaj.Network.Packets.Status.Json;

namespace blahaj.blahaj.Player;

public class MinecraftPlayer
{
    public string Name { get; set; } = "";
    public Guid? Uuid { get; set; }
    public Guid SessionId { get; private set; } = Guid.NewGuid();
    public ClientInformationPacket ClientInformation { get; set; }

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

    public StatusPlayer ToStatus()
    {
        return new StatusPlayer(Name, Uuid.ToString());
    }
}