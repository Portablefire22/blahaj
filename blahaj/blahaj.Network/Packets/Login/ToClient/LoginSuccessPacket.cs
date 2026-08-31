using blahaj.blahaj.Network.Packets.Login.ToClient.Json;
using blahaj.blahaj.Network.Packets.Play.PlayerInfo;
using blahaj.blahaj.Stream;

namespace blahaj.blahaj.Network.Packets.Login.ToClient;

[PacketState(ConnectionState.Login)]
[PacketDirection(PacketDirection.ServerToClient)]
public class LoginSuccessPacket : Packet
{
    public Guid Uuid { get; private set; }
    public string Username { get; private set; }
    public Guid SessionId { get; private set; }
    public GameProfileProperties[] Properties { get; private set; }

    public LoginSuccessPacket() : base(0x02)
    {
    }

    public LoginSuccessPacket(LoginSuccessJson json, Guid sessionId) : this()
    {
        Uuid = Guid.Parse(json.Id);
        Username = json.Name;
        Properties = json.Properties;
        SessionId = sessionId;
    }

    public override void Read(MinecraftStream stream)
    {
        throw new NotImplementedException();
    }

    public override void Write(MinecraftStream stream)
    {
        stream.WriteUuid(Uuid);
        stream.WriteString(Username);
        using var ms = new MemoryStream();
        using var mine = new MinecraftStream(ms);
        Properties = new GameProfileProperties[] { };
        foreach (var property in Properties)
        {
            mine.WriteString(property.Name);
            mine.WriteString(property.Value);
            mine.WritePrefixedOptionalString(property.Signature);
        }
        var dat = ms.ToArray();
        stream.WriteVarInt(dat.Length);
        stream.WriteByteArray(dat);
        stream.WriteUuid(SessionId);
    }
}