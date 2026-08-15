using System.Text.Json;
using blahaj.blahaj.Network.Packets.Status.ToClient.Json;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Status.ToClient;

[PacketState(ConnectionState.Status)]
[PacketDirection(PacketDirection.ServerToClient)]
public class StatusResponsePacket : Packet
{
    private StatusResponse Status;
    
    public StatusResponsePacket(StatusResponse status) : this()
    {
        Status = status;
    }

    public StatusResponsePacket() : base(0x0)
    {
    }

    public override void Read(MinecraftStream stream)
    {
        throw new NotImplementedException();
    }

    public override void Write(MinecraftStream stream)
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        stream.WriteString(JsonSerializer.Serialize(Status, jsonOptions));
    }
}