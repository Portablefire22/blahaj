using blahaj.blahaj.Network.Packets.Play.PlayerInfo;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Play.ToClient;

[PacketDirection(PacketDirection.ServerToClient)]
[PacketState(ConnectionState.Play)]
public class PlayerInfoUpdate : Packet
{
    public PlayerInfoUpdate() : base(0x46)
    {
    }

    public byte Action { get; set; }
    
    public KeyValuePair<Guid, IPlayerInfoAction[]>[] Players { get; set; }
    
    public override void Read(MinecraftStream stream)
    {
        throw new NotImplementedException();
    }

    public override void Write(MinecraftStream stream)
    {
        
        stream.WriteUnsignedByte(Action);
        stream.WriteVarInt(Players.Length);
        foreach (var (uuid, actions) in Players)
        {
            stream.WriteUuid(uuid);
            foreach (var action in actions)
            {
                action.Write(stream);
            }
        }
    }
}
