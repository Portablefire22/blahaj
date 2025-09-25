using System.Text.Json;
using blahaj.blahaj.Stream;
using blahaj.Network.Packets.Status.Json;

namespace blahaj.Network.Packets.Status;

public class StatusResponsePacket : Packet
{
    private StatusResponse Status;
    
    public StatusResponsePacket(StatusResponse status) : base(0x0)
    {
        Status = status;
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