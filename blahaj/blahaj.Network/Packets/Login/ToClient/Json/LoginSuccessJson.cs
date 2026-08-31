using blahaj.blahaj.Network.Packets.Play.PlayerInfo;

namespace blahaj.blahaj.Network.Packets.Login.ToClient.Json;

public class LoginSuccessJson
{
    public string Id { get; set; }
    public string Name { get; set; }
    public GameProfileProperties[] Properties { get; set; }
    
    
    public GameProfile ToProfile()
    {
        return new GameProfile()
        {
            Uuid = Guid.Parse(Id),
            Username = Name,
            Properties = Properties
        };
    }
    
    
}