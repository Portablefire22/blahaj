using blahaj.blahaj.Network.Packets.Interfaces;
using blahaj.blahaj.Network.Packets.Status.ToClient;
using blahaj.blahaj.Network.Packets.Status.ToClient.Json;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Status.ToServer;

[PacketState(ConnectionState.Status)]
[PacketDirection(PacketDirection.ClientToServer)]
public class StatusRequestPacket : Packet, IInvokableWithServer
{
    public StatusRequestPacket() : base(0){}
    
    public override void Read(MinecraftStream stream)
    {
        // Has no data
    }
    
    public override void Write(MinecraftStream stream)
    {
    }

    public Packet? Invoke()
    {
        // Maybe I should check for nulls, or maybe the user should just set up configs correctly
        var version = new StatusVersion(Server.Config["version"], short.Parse(Server.Config["protocol"]));
        // Get all connections that contain a valid player
        var validPlayers = Server.Players.Where(x => x.IsValid()).ToArray();
        // Get all players that want to show in the listing
        var temp = validPlayers.Where(x => x.ClientInformation.AllowServerListings).Select(x => 
            x.ToStatus()).ToArray();
        var players = new StatusPlayers(int.Parse(Server.Config["maxPlayers"]), validPlayers.Length,
            temp.Length > 0 ? temp : []);
        var desc = new StatusDescription(Server.Config["motd"]);
        
        var resp = new StatusResponse(version, players, desc, $"data:image/png;base64,{Server.Favicon}", 
            bool.Parse(Server.Config["enforcesSecureChat"]));
        return new StatusResponsePacket(resp);
    }

    public NetServer Server { get; set; }
}